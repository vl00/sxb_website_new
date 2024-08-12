using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate
{
    public enum PointsTaskType
    {
        /// <summary>
        /// 日常任务
        /// </summary>
        DayTask,
        /// <summary>
        /// 运营任务
        /// </summary>
        OperationTask 
    }
}
