using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Queries.UserPointsTask
{
    public record UserTaskStatusViewModel
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// 每天最多完成次数
        /// </summary>
        public byte MaxTimesEveryDay { get; set; }

        /// <summary>
        /// 完成总次数
        /// </summary>
        public byte FinishTimes { get; set; }

        /// <summary>
        /// 今日完成总次数
        /// </summary>
        public byte FinishTimesToday { get; set; }

        /// <summary>
        /// 最多可完成次数
        /// </summary>
        public long MaxTimes { get; set; }

        /// <summary>
        /// 今天是否已完成
        /// </summary>
        public bool IsFinish
        {
            get
            {
                if (FinishTimes == MaxTimes)
                    return true;
                if (FinishTimesToday == MaxTimesEveryDay)
                    return true;
                return false;
            }
        }
    }
}
