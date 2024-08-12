using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.API.Application.Commands;
using Sxb.PointsMall.API.Application.Queries.AccountPoints;
using Sxb.PointsMall.API.Application.Queries.UserPointsTask;
using Sxb.PointsMall.API.Application.Queries.UserSignInInfo;
using Sxb.PointsMall.API.Models;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using Sxb.PointsMall.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sxb.Framework.Foundation;
using System.ComponentModel;
using Sxb.PointsMall.API.Config;
using Sxb.Framework.Cache.Redis;
using Microsoft.AspNetCore.Mvc.Filters;
using Sxb.PointsMall.API.Infrastructure.Filter;
using Sxb.PointsMall.Domain.Events;
using Sxb.PointsMall.API.Application.IntegrationEvents;
using DotNetCore.CAP;
using Sxb.PointsMall.API.Infrastructure.Services;

namespace Sxb.PointsMall.API.Controllers
{

    /// <summary>
    /// 任务中心
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserPointsTaskController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICapPublisher _capPublisher;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ILogger<UserPointsTaskController> _logger;

        private readonly IUserService _userService;
        private readonly IUserPointsTaskQueries _userPointsTaskQueries;

        public UserPointsTaskController(IMediator mediator, ICapPublisher capPublisher, IEasyRedisClient easyRedisClient, ILogger<UserPointsTaskController> logger, IUserPointsTaskQueries userPointsTaskQueries, IUserService userService)
        {
            _mediator = mediator;
            _capPublisher = capPublisher;
            _easyRedisClient = easyRedisClient;
            _logger = logger;
            _userPointsTaskQueries = userPointsTaskQueries;
            _userService = userService;
        }

        [HttpGet("RSA")]
        public async Task<ResponseResult> GetRSAKeyPair()
        {
            return ResponseResult.Success(RSAHelper.GenerateRSAKeyPair());
        }

        [HttpGet("Grant")]
        [Authorize]
        public async Task<ResponseResult> GrantAsync(int taskId)
        {
            return await _mediator.Send(new GrantTaskCommand()
            {
                UserId = User.Identity.GetID(),
                TaskId = taskId
            });
        }

        /// <summary>
        /// 2 领取浏览种草任务
        /// </summary>
        /// <remarks>备注</remarks>
        /// <returns></returns>
        [HttpGet("Take/ViewEvaluation")]
        [Authorize]
        public async Task<ResponseResult> GetViewEvaluationTaskAsync()
        {
            //return GetCurrentAsync(2);

            var userId = User.Identity.GetID();
            int taskId = PointsTask.ViewEvaluationTaskId;
            var userPointsTask = await _userPointsTaskQueries.GetCurrentAsync(userId, taskId);
            if (userPointsTask == null)
            {
                return ResponseResult.Failed("无任务");
            }
            var isFinish = userPointsTask.IsFinish;
            if (isFinish)
            {
                return ResponseResult.Success(new
                {
                    isFinish
                });
            }


            var startTime = DateTime.Now;
            var redisKey = string.Format(RedisKeys.TakeViewEvaluationTaskEncrptKey, userId);
            var succeed = await _easyRedisClient.AddAsync(redisKey, startTime, TimeSpan.FromHours(24));

            if (!succeed)
            {
                return ResponseResult.Failed("系统错误");
            }

            return ResponseResult.Success(new
            {
                key = MD5Helper.GetMD5(redisKey + DateTime.Now.ToUnixTimestampBySeconds()).ToLower(),
                startTime,
                isFinish
            });
        }

        /// <summary>
        /// 2 完成浏览种草任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("TakeAndFinish/ViewEvaluation")]
        [Authorize]
        [SignatureFilter(signatureUserId: true)]
        public async Task<ResponseResult> TakeAndFinishViewEvaluationTaskAsync(string key, long timestamp, string signature)
        {
            var userId = User.Identity.GetID();

            var redisKey = string.Format(RedisKeys.TakeViewEvaluationTaskEncrptKey, userId);
            var startTime = await _easyRedisClient.GetAsync<DateTime?>(redisKey);
            if (startTime == null)
            {
                return ResponseResult.Failed("未领取任务");
            }
            if (DateTime.Now - startTime.Value < TimeSpan.FromSeconds(30))
            {
                return ResponseResult.Failed("不足30秒");
            }


            //return FinishTaskAsync(2)
            //当天用户任务id
            int taskId = 2;
            return await _mediator.Send(new CreateAndFinishTaskCommand()
            {
                UserId = userId,
                TaskId = taskId,
                FromId = string.Empty
            });
        }

        /// <summary>
        /// 3 领取并完成分享上学帮商城任务
        /// </summary>
        /// <param name="userId">分享人</param>
        /// <param name="time">分享时间</param>
        /// <param name="timestamp"></param>
        /// <param name="signature">signature</param>
        /// <returns></returns>
        [HttpGet("TakeAndFinish/Share")]
        [Authorize]
        [SignatureFilter]
        public async Task<ResponseResult> TakeAndFinishShareTaskAsync(Guid userId, string time, long timestamp, string signature)
        {
            var shareTime = DateTime.ParseExact(time, "yyyyMMdd", null);


            int taskId = PointsTask.ShareTaskId;
            //userId = Guid.Parse("{FCBB302A-097D-4664-89E3-B659F8D62B92}");
            //点击分享的人的id
            var clickUserId = User.Identity.GetID();
            if (shareTime.Date != DateTime.Today.Date)
            {
                return ResponseResult.Failed("只能完成当天的任务");
            }
            //去完成
            return await _mediator.Send(new CreateAndFinishTaskCommand()
            {
                UserId = userId,
                TaskId = taskId,
                FromId = clickUserId == default ? string.Empty : clickUserId.ToString()
            });
        }

        /// <summary>
        /// 接取任务
        /// </summary>
        /// <param name="taskId">任务id</param>
        /// <returns></returns>
        [HttpGet("Current")]
        [Authorize]
        public async Task<ResponseResult<UserTaskStatusViewModel>> GetCurrentAsync(int taskId)
        {
            var userId = User.Identity.GetID();
            //userId = Guid.Parse("{FCBB302A-097D-4664-89E3-B659F8D62B92}");

            //当天用户任务id
            var userPointsTask = await _userPointsTaskQueries.GetCurrentAsync(userId, taskId);
            if (userPointsTask == null)
            {
                return ResponseResult<UserTaskStatusViewModel>.Failed("无此任务");
            }

            if (userPointsTask.IsFinish)
            {
                return ResponseResult<UserTaskStatusViewModel>.Failed("已完成今日任务");
            }
            return ResponseResult<UserTaskStatusViewModel>.Success(userPointsTask, "");
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <returns></returns>
        //[HttpGet("Finish")]
        //[Authorize]
        //public async Task<ResponseResult> FinishTaskAsync(int taskId)
        //{
        //    var userId = User.Identity.GetID();
        //    //userId = Guid.Parse("{FCBB302A-097D-4664-89E3-B659F8D62B92}");

        //    //去完成
        //    return await _mediator.Send(new FinishTaskCommand()
        //    {
        //        UserId = userId,
        //        TaskId = taskId
        //    });
        //}



        [HttpPost("Test/PublishAddEvaluation")]
        public async Task<ResponseResult> TestPublishAddEvaluationAsync([FromBody] AddEvaluationIntegrationEvent evt)
        {
            await _capPublisher.PublishAsync(nameof(AddEvaluationIntegrationEvent), evt);
            return ResponseResult.Success();
        }

        [HttpPost("Test/PublishAddChild")]
        public async Task<ResponseResult> TestPublishAddChildAsync([FromBody] AddChildIntegrationEvent evt)
        {
            await _capPublisher.PublishAsync(nameof(AddChildIntegrationEvent), evt);
            return ResponseResult.Success();
        }


        /// <summary>
        /// 3 领取并完成分享关注任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("TakeAndFinish/WeChatSubcribe")]
        [Authorize]
        public async Task<ResponseResult> TakeAndFinishWeChatSubcribeTaskAsync(string openId)
        {
            var userId = User.Identity.GetID();
            var taskId = PointsTask.WeChatSubcribeTaskId;

            var isSubscribe = await _userService.GetUserSubscribe(userId);
            if (!isSubscribe)
            {
                return ResponseResult.Failed("未关注");
            }

            //去完成
            return await _mediator.Send(new CreateAndFinishTaskCommand()
            {
                UserId = userId,
                TaskId = taskId,
                FromId = userId.ToString(),
                Remark = openId
            });
        }
    }
}
