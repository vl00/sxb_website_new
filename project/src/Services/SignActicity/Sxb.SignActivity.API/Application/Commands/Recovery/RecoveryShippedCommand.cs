using MediatR;
using System;

namespace Sxb.SignActivity.API.Application.Commands
{
    public class RecoveryShippedCommand : IRequest<object>
    {
        /// <summary>
        /// 是否补发签到请求
        /// </summary>
        public bool IsSend { get; set; } = false;
        public DateTime? SignInDate { get;  set; }
    }
}
