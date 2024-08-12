using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System;

namespace Sxb.SignActivity.API.Application.Commands
{
    public class OrderShippedCommand : IRequest<ResponseResult>
    {
        public Guid UserId { get; set; }
        /// <summary>
        /// 优先使用orderId
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// 如果orderId = Empty, 则按天解锁
        /// </summary>
        public DateTime? SignInDate { get; set; }

        /// <summary>
        /// 是否不检查签到金额, 直接解冻
        /// </summary>
        public bool UnCheckSignAmount { get; set; } = false;
    }
}
