using MediatR;
using Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class DeductFreezePointsCommand : IRequest
    {
        /// <summary>
        /// 冻结Id，冻结的时候能取到该ID
        /// </summary>

        public Guid FreezeId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 来源类型
        /// </summary>
        public AccountPointsOriginType OriginType { get; set; }
    }
}
