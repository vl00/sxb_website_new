using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate
{
    public enum AccountPointsItemState
    {

        /// <summary>
        /// 正常状态
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 冻结状态
        /// </summary>
        Freeze =1,
    }
}
