using MediatR;
using System;

namespace Sxb.SignActivity.API.Application.Commands
{
    public class RecoverySignInCommand : IRequest<object>
    {
        public DateTime? SignInDate { get; set; }

        /// <summary>
        /// 是否补发签到请求
        /// </summary>
        public bool IsSend { get; set; } = false;


        public bool Parent { get; set; } = false;
    }
}
