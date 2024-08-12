using MediatR;
using Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{

    /// <summary>
    /// 加冻结积分
    /// </summary>
    public class AddAccountFreezePointsCommand : IRequest<Guid>
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 需要加的积分
        /// </summary>
        public long FreezePoints { get; set; }

        /// <summary>
        /// 来源ID
        /// </summary>
        public string OriginId { get; set; }

        /// <summary>
        /// 备注（简述为什么加。）
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 积分来源类型
        /// </summary>
        public AccountPointsOriginType OriginType { get; set; }
    }
}
