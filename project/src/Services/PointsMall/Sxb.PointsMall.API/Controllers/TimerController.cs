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

namespace Sxb.PointsMall.API.Controllers
{

    /// <summary>
    /// 定时任务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TimerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICapPublisher _capPublisher;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ILogger<TimerController> _logger;

        private readonly IUserSignInInfoQueries _userSignInInfoQueries;

        public TimerController(IMediator mediator, ICapPublisher capPublisher, IEasyRedisClient easyRedisClient, ILogger<TimerController> logger, IUserSignInInfoQueries userSignInInfoQueries)
        {
            _mediator = mediator;
            _capPublisher = capPublisher;
            _easyRedisClient = easyRedisClient;
            _logger = logger;
            _userSignInInfoQueries = userSignInInfoQueries;
        }

        [HttpGet("NotifySignIn")]
        public async Task<ResponseResult> NotifySignInAsync()
        {
            _logger.LogInformation("开始签到提醒{0}", DateTime.Now.Ticks);

            var notifyUsers = await _userSignInInfoQueries.GetNotifyUsers();
            _logger.LogInformation("签到提醒数据,data={0}", notifyUsers.ToJson());

            var templateSettingCode = "PointsMall:SignIn:Nofify";

            var type = typeof(NotifyUserInfoViewModel);
            foreach (var item in notifyUsers)
            {
                //当日未签到, 提醒签到
                if (!item.IsSignToday)
                {
                    _capPublisher.Publish(nameof(SendMsgIntegrationEvent), new SendMsgIntegrationEvent(
                        templateSettingCode,
                        item.UserId,
                        CommonHelper.ToDictionary(type, item)
                    ));
                }
            }

            _logger.LogInformation("结束签到提醒{0}", DateTime.Now.Ticks);
            return ResponseResult.Success();
        }



        [HttpGet("NotifyPointExpire")]
        public async Task<ResponseResult> NotifyPointExpireAsync(
            [FromServices] IAccountPointsQueries accountPointsQueries
            )
        {
            var notifyUsers = await accountPointsQueries.GetPointsOverZeroUserIds();

            var templateSettingCode = "PointsMall:PointsExpire:Nofify";

            foreach (var item in notifyUsers)
            {
                //当日未签到, 提醒签到
                _capPublisher.Publish(nameof(SendMsgIntegrationEvent), new SendMsgIntegrationEvent(
                templateSettingCode,
                item,
                null));
            }

            return ResponseResult.Success();
        }

    }
}
