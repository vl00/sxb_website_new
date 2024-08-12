using Newtonsoft.Json;
using Sxb.Domain;
using Sxb.PointsMall.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate
{
    /// <summary>
    /// 积分任务
    /// </summary>
    public class PointsTask : Entity<int>
    {
        public static int SignInTaskId = 1;
        public static int ViewEvaluationTaskId = 2;
        public static int ShareTaskId = 3;
        public static int OrderTaskId = 4;
        public static int AddEvaluatinTaskId = 5;
        public static int AddChildTaskId = 6;
        public static int WeChatSubcribeTaskId = 7;

        public string Name { get; private set; }
        public PointsTaskType Type { get; private set; }
        /// <summary>
        /// 完成一次可获得积分
        /// </summary>
        public int TimesPoints { get; private set; }
        /// <summary>
        /// 连续签到积分
        /// </summary>
        public string TimesPointsList { get; private set; }
        public byte MaxTimesEveryDay { get; private set; }
        public string Desc { get; private set; }
        public long MaxTimes { get; private set; }

        public bool IsEnable { get; private set; }


        public PointsTask(int id, string name, PointsTaskType type, int timesPoints, byte maxTimesEveryDay, string desc, int maxTimes)
        {

            this.Id = id;
            this.Name = name;
            this.Type = type;
            this.TimesPoints = timesPoints;
            this.MaxTimesEveryDay = maxTimesEveryDay;
            this.Desc = desc;
            this.MaxTimes = maxTimes;
        }

        private PointsTask()
        {
        }

        public int GetPoints(int days)
        {
            //--------非签到--------
            if (Id != SignInTaskId)
            {
                return TimesPoints;
            }

            //--------签到--------
            if (string.IsNullOrWhiteSpace(TimesPointsList))
            {
                throw new Exception("签到积分配置为空");
            }
            var points = JsonConvert.DeserializeObject<int[]>(TimesPointsList);
            if (days > points.Length)
            {
                throw new Exception("签到积分配置数量不足");
            }
            if (days < 1)
            {
                throw new Exception("签到天数错误, 至少一天");
            }
            return points[days - 1];
        }
    }
}
