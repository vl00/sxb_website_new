using DotNetCore.CAP;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.SignActivity.API.Application.Commands;
using Sxb.SignActivity.API.Application.IntegrationEvents;
using Sxb.SignActivity.API.Application.Query;
using Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter;
using Sxb.SignActivity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.SignActivity.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICapPublisher _capPublisher;
        private readonly ILogger<SignController> _logger;
        private readonly ISignQuery _signQuery;
        private readonly IFinanceCenterAPIClient _financeCenterAPIClient;

        public SignController(IMediator mediator, ILogger<SignController> logger, ISignQuery signQuery, ICapPublisher capPublisher, IFinanceCenterAPIClient financeCenterAPIClient)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signQuery = signQuery ?? throw new ArgumentNullException(nameof(signQuery));
            _capPublisher = capPublisher;
            _financeCenterAPIClient = financeCenterAPIClient;
        }

        [HttpGet("")]
        public async Task<ResponseResult> GetAll()
        {
            var data = await _signQuery.GetSignInsAsync();
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpGet("SignIn")]
        public async Task<ResponseResult> SignIn([FromQuery] SignInCommand command)
        {
            return await _mediator.Send(command);
        }

        /// <summary>
        /// 签到父级
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpGet("SignInParent")]
        public async Task<ResponseResult> SignInParent([FromQuery] SignInParentCommand command)
        {
            return await _mediator.Send(command);
        }

        /// <summary>
        /// 收货后解冻
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpGet("Shipped")]
        public async Task<ResponseResult> Shipped([FromQuery] OrderShippedCommand command)
        {
            return await _mediator.Send(command);
        }

        /// <summary>
        /// 比对Order和SignIn恢复漏签到
        /// </summary>
        /// <returns></returns>
        [HttpGet("RecoverySignIn")]
        public async Task<ResponseResult> RecoverySignIn([FromQuery] RecoverySignInCommand command)
        {
            var data = await _mediator.Send(command);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 比对SignIn恢复Parent,InviteCount
        /// </summary>
        /// <returns></returns>
        [HttpGet("RecoveryParent")]
        public async Task<ResponseResult> RecoveryParent([FromQuery] RecoveryParentCommand command)
        {
            var data = await _mediator.Send(command);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 比对Order和SignInHistory恢复漏解冻
        /// </summary>
        /// <returns></returns>
        [HttpGet("RecoveryShipped")]
        public async Task<ResponseResult> RecoveryShipped([FromQuery] RecoveryShippedCommand command)
        {
            var data = await _mediator.Send(command);
            return ResponseResult.Success(data);
        }


        /// <summary>
        /// 发送一条信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [HttpGet("SendMessage")]
        public async Task<ResponseResult> SendMessage(Guid userId, string msg)
        {
            if (userId == Guid.Empty) userId = Guid.Parse("FCBB302A-097D-4664-89E3-B659F8D62B92");
            if (string.IsNullOrWhiteSpace(msg)) msg = "测试消息" + DateTime.Now.Ticks;

            await _capPublisher.PublishAsync(nameof(WeChatMsgIntegrationEvent), new WeChatMsgIntegrationEvent()
            {
                UserId = userId,
                Msg = msg
            });

            return ResponseResult.Success();
        }

        /// <summary>
        /// 发送签到提醒
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpGet("SendReminds")]
        public async Task<ResponseResult> SendReminds([FromQuery] SignInRemindCommand command)
        {
            var data = await _mediator.Send(command);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 解冻冻结金额
        /// </summary>
        /// <param name="freezeMoneyInLogId"></param>
        /// <returns></returns>
        [HttpGet("UnFreezeMoney")]
        public async Task<APIResult<string>> UnFreezeMoney(string freezeMoneyInLogId)
        {
            var resp = await _financeCenterAPIClient.InsideUnFreezeAmount(freezeMoneyInLogId);
            return resp;
        }
    }
}
