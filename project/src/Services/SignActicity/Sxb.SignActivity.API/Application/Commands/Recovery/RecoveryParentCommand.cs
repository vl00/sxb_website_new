using MediatR;
using System;

namespace Sxb.SignActivity.API.Application.Commands
{
    public class RecoveryParentCommand : IRequest<object>
    {
        /// <summary>
        /// 是否补发签到请求
        /// </summary>
        public bool IsSend { get; set; } = false;
    }
}
