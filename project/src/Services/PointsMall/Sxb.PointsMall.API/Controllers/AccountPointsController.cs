using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.API.Application.Commands;
using Sxb.PointsMall.API.Application.Models;
using Sxb.PointsMall.API.Application.Queries.AccountPoints;
using Sxb.PointsMall.API.Application.Queries.AccountPointsItem;
using Sxb.PointsMall.API.Application.Queries.UserPointsTask;
using Sxb.PointsMall.API.Application.Queries.UserSignInInfo;
using Sxb.PointsMall.API.Filters;
using Sxb.PointsMall.API.Infrastructure.Services;
using Sxb.PointsMall.API.Models;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using Sxb.PointsMall.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Controllers
{
    [Description("积分")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountPointsController : ControllerBase
    {
        IMediator _mediator;
        private readonly ILogger<AccountPointsController> _logger;
        private readonly IUserService _userService;
        public AccountPointsController(IMediator mediator, IUserService userService, ILogger<AccountPointsController> logger)
        {
            _mediator = mediator;
            _userService = userService;
            _logger = logger;
        }


        /// <summary>
        /// 个人积分任务聚合页面详情：
        ///     不仅仅包含积分，还包含积分任务、积分商品等信息。
        /// </summary>
        /// <returns></returns>
        [HttpGet("detail")]
        [Authorize]
        public async Task<ResponseResult> PointsDetail(
            [FromServices] IUserSignInInfoQueries userSignInInfoQueries,
            [FromServices] IAccountPointsQueries accountPointsQueries,
            [FromServices] IUserPointsTaskQueries userPointsTaskQueries
            )
        {
            PointsDetailData pointsDetailData = new PointsDetailData();
            var userId = HttpContext.GetUserInfo().UserId;
            pointsDetailData.AccountPoints = await accountPointsQueries.GetAccountPoints(userId);
            pointsDetailData.UserSignInInfo = await userSignInInfoQueries.GetUserSignInInfo(userId);

            pointsDetailData.UserPointsTaskItems = await userPointsTaskQueries.GetPointsTasksOfUser(userId);
            var isSubscribe = await CheckSubscribe(pointsDetailData.UserPointsTaskItems, userId);
            //取关后, 把提醒状态改为取消
            if (!isSubscribe)
            {
                pointsDetailData.UserSignInInfo.EnableNotify = false;
                await SetSignInNotify(false);
            }

            return ResponseResult.Success(pointsDetailData);
        }

        /// <summary>
        /// 对关注任务进行补偿
        /// 1.对已关注老用户补偿
        /// 2,对新关注回调失败
        /// </summary>
        /// <param name="userPointsTaskItems"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<bool> CheckSubscribe(IEnumerable<PointsTasksOfUser> userPointsTaskItems, Guid userId)
        {
            var isSubscribe = false;
            try
            {
                isSubscribe = await _userService.GetUserSubscribe(userId);
                if (!isSubscribe)
                {
                    return isSubscribe;
                }

                var taskId = PointsTask.WeChatSubcribeTaskId;
                var subscribeTask = userPointsTaskItems.FirstOrDefault(s => s.Id == taskId);
                if (subscribeTask.IsFinish)
                {
                    return isSubscribe;
                }

                var result = await _mediator.Send(new CreateAndFinishTaskCommand()
                {
                    UserId = userId,
                    TaskId = taskId,
                    FromId = userId.ToString(),
                    Remark = "补偿关注任务"
                });
                if (!result.Succeed)
                {
                    _logger.LogError("补偿关注任务错误:{0}", result.Msg);
                }
            }
            catch (Exception)
            { }
            return isSubscribe;
        }



        /// <summary>
        /// 每日签到功能
        /// </summary>
        /// <returns></returns>
        [HttpGet("daySignIn")]
        [Authorize]
        public async Task<ResponseResult> DaySignIn()
        {

            SignInCommand cmd = new SignInCommand() { UserId = HttpContext.GetUserInfo().UserId };
            try
            {
                if (await _mediator.Send(cmd))
                    return ResponseResult.Success("签到成功");
                else
                    return ResponseResult.Failed("签到失败");
            }
            catch (DaySignInException ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }


        /// <summary>
        /// 获取积分明细列表
        /// </summary>
        /// <returns></returns>
        [Description("获取积分明细列表")]
        [HttpGet("AccountPointsDetails")]
        [Authorize]
        public async Task<ResponseResult<AccountPointsDetails>> GetAccountPointsDetails([FromServices] IAccountPointsItemQueries accountPointsItemQueries, [FromQuery] GetAccountPointsDetailsFilter filter)
        {
            filter.UserId = HttpContext.GetUserInfo().UserId;
            var accountPointsDetails = await accountPointsItemQueries.GetAccountPointsDetails(filter);
            return ResponseResult<AccountPointsDetails>.Success(accountPointsDetails, "OK");
        }


        [Description("获取个人主页滚动任务列表")]
        [HttpGet("ScrollPointsTasks")]
        [Authorize]
        public async Task<ResponseResult<IEnumerable<PointsTasksOfUser>>> GetScrollPointsTasksOfUser(
            [FromServices] IUserPointsTaskQueries userPointsTaskQueries)
        {
            var pointsTasksOfUser = await userPointsTaskQueries.GetScrollPointsTasksOfUser(HttpContext.GetUserInfo().UserId);
            await CheckSubscribe(pointsTasksOfUser, HttpContext.GetUserInfo().UserId);
            return ResponseResult<IEnumerable<PointsTasksOfUser>>.Success(pointsTasksOfUser, "OK");
        }


        /// <summary>
        /// 冻结积分
        /// </summary>
        /// <remarks>冻结积分，产生一条冻结明细。</remarks>
        /// <returns></returns>
        [HttpPost("FreezePoints")]
        [InnerAuthorize]
        public async Task<ResponseResult> FreezePoints(FreezePointsCommand cmd)
        {
            var freezeeId = await _mediator.Send(cmd);
            return ResponseResult.Success(new { FreezeId = freezeeId });
        }

        /// <summary>
        /// 加冻结积分
        /// </summary>
        /// <remarks>冻结积分，产生一条冻结明细。</remarks>
        /// <returns></returns>
        [HttpPost("AddFreezePoints")]
        [InnerAuthorize]
        public async Task<ResponseResult> AddFreezePoints(AddAccountFreezePointsCommand cmd)
        {
            var freezeeId = await _mediator.Send(cmd);
            return ResponseResult.Success(new { FreezeId = freezeeId });
        }

        /// <summary>
        /// 解冻积分
        /// </summary>
        /// <remarks>从账户的冻结积分转为用户积分。</remarks>
        /// <returns></returns>
        [HttpPost("DeFreezePoints")]
        [InnerAuthorize]
        public async Task<ResponseResult> DeFreezePoints(DeFreezePointsCommand cmd)
        {
            await _mediator.Send(cmd);
            return ResponseResult.Success();
        }

        /// <summary>
        /// 扣除冻结积分
        /// </summary>
        /// <remarks>从账户的冻结积分中扣除指积分，并且产生一条扣除明细，原来处于冻结状态也会变为正常状态。</remarks>
        /// <returns></returns>
        [HttpPost("DeductFreezePoints")]
        [InnerAuthorize]
        public async Task<ResponseResult> DeductFreezePoints(DeductFreezePointsCommand cmd)
        {
            await _mediator.Send(cmd);
            return ResponseResult.Success();
        }

        /// <summary>
        /// 加账户积分
        /// </summary>
        /// <remarks>完成某项业务加商城积分。</remarks>
        /// <returns></returns>
        [HttpPost("AddAccountPoints")]
        [InnerAuthorize]
        public async Task<ResponseResult> AddAccountPoints(AddAccountPointsCommand cmd)
        {
            await _mediator.Send(cmd);
            return ResponseResult.Success();
        }

        /// <summary>
        /// 切换签到提醒状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("SignIn/Notify")]
        [Authorize]
        public async Task<ResponseResult> SetSignInNotify(bool enabled)
        {
            return await _mediator.Send(new SignInNotifyCommand()
            {
                UserId = User.Identity.GetID(),
                Enabled = enabled
            });
        }


    }
}
