using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System;
using System.Collections.Generic;

namespace Sxb.SignActivity.API.Application.Commands
{
    public class SignInRemindCommand : IRequest<ResponseResult>
    {
        /// <summary>
        /// 不指定则, 全量提醒
        /// </summary>
        public IEnumerable<Guid> UserIds { get; set; }

        /// <summary>
        /// 是否发送模板消息
        /// </summary>
        public bool IsSendMsg { get; set; }
    }
}