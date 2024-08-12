using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Queries.UserPointsTask
{
    public record PointsTasksOfUser
    {

        [Description("任务枚举ID")]
        public int Id { get; set; }

        [Description("任务昵称")]
        public string Name { get; set; }

        [Description("任务描述")]
        public string Desc { get; set; }

        [Description("完成一次可获得积分")]
        public int TimesPoints { get; set; }

        [Description("每天最多完成次数")]
        public byte MaxTimesEveryDay { get; set; }

        [Description("完成总次数")]
        public byte FinishTimes { get; set; }

        [Description("今日完成总次数")]
        public byte FinishTimesToday { get; set; }

        [Description("今日待领取总次数")]
        public byte WaitGrantTimesToday { get; set; }

        [Description("待领取总次数")]
        public byte WaitGrantTimes { get; set; }

        /// <summary>
        /// 任务类型 0 日常任务  1 运营任务
        /// </summary>
        [Description("任务类型")]
        public byte Type { get; set; }

        [Description("最多可完成次数")]
        public long MaxTimes { get; set; }

        [Description("今天是否已完成")]
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

        [Description("完成状态")]
        public TaskStatus Status
        {
            get
            {
                //已完成历史总计任务数量
                if (FinishTimes > 0 && FinishTimes >= MaxTimes)
                {
                    //日常任务当日全部领取
                    if (Type == 0 && WaitGrantTimesToday == 0)
                    {
                        return TaskStatus.AllyGranted;
                    }
                    //运营任务历史全部领取
                    else if(Type == 1 && WaitGrantTimes == 0)
                    {
                        return TaskStatus.AllyGranted;
                    }
                    return TaskStatus.PartiallyGranted;
                }


                if (FinishTimesToday == 0)
                {
                    return TaskStatus.UnFinish;
                }

                //全部任务完成
                if (FinishTimesToday >= MaxTimesEveryDay)
                {
                    //已全部完成, 全部积分已领取
                    if (WaitGrantTimesToday == 0)
                    {
                        return TaskStatus.AllyGranted;
                    }
                    else
                    {
                        //有待领取积分
                        return TaskStatus.PartiallyGranted;
                    }
                }
                else
                {
                    //部分完成, 已领取
                    if (WaitGrantTimesToday == 0)
                    {
                        return TaskStatus.UnFinish;
                    }
                    //部分完成, 待领取
                    else
                    {
                        //有待领取积分
                        return TaskStatus.PartiallyGranted;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        public enum TaskStatus
        {
            UnFinish = 0,
       
            PartiallyGranted = 1,
            AllyGranted = 2
,
        }
    }
}
