using MediatR;
using Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    /// <summary>
    /// 解冻积分
    /// </summary>
    public class DeFreezePointsCommand : IRequest
    {

        /// <summary>
        /// 冻结Id，冻结的时候能取到该ID
        /// </summary>

        public Guid FreezeId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }


    }
}
