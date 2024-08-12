using MediatR;
using Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    /// <summary>
    /// 冻结积分,返回一个冻结ID
    /// </summary>
    public class FreezePointsCommand:IRequest<Guid>
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 需要冻结的积分
        /// </summary>
        public long FreezePoints { get; set; }

        public string OriginId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 积分来源类型
        /// </summary>

        public AccountPointsOriginType  OriginType { get; set; }

    }
}
