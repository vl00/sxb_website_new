using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;

namespace Sxb.PointsMall.API.Models.EntityInfo
{
    public class PointsTaskInfo
    {
        public string Name { get; private set; }
        public PointsTaskType Type { get; private set; }
        /// <summary>
        /// 完成一次可获得积分
        /// </summary>
        public int TimesPoints { get; private set; }
        public byte MaxTimesEveryDay { get; private set; }
        public string Desc { get; private set; }
        public long MaxTimes { get; private set; }

        public bool IsEnable { get; private set; }
    }
}
