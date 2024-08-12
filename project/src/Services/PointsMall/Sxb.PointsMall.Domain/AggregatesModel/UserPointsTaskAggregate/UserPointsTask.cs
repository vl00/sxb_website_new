using MediatR;
using Sxb.Domain;
using Sxb.PointsMall.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate
{
    /// <summary>
    /// 用户积分任务
    /// </summary>
    public class UserPointsTask : Entity<Guid>, IAggregateRoot
    {
        public Guid UserId { get; private set; }

        public PointsTask PointsTask { get; set; }

        public int PointsTaskId => PointsTask.Id;

        /// <summary>
        /// 获得积分
        /// </summary>
        public int GetPoints { get; private set; }

        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// 任务最后的完成时间（适应一条可以多次完成的任务,记录最后的完成时间。）
        /// </summary>
        /// 
        public DateTime? EndFinishTime { get; private set; }

        /// <summary>
        /// 状态  0 为创建/未完成  1 已完成,未领取积分 2 已领取积分
        /// </summary>
        public UserPointsTaskStatus Status { get; private set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// 关联id 分享 -> 点击人userid,下单 -> 订单id,种草 -> 种草id
        /// </summary>
        public string FromId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public UserPointsTask(Guid id, Guid uesrId, PointsTask pointsTask, int getPoints, DateTime createTime, DateTime? endFinishTime)
        {
            this.Id = id;
            this.UserId = uesrId;
            this.PointsTask = pointsTask;
            this.GetPoints = getPoints; // pointsTask.TimesPoints;
            this.CreateTime = createTime;
            this.EndFinishTime = endFinishTime;
            Status = UserPointsTaskStatus.UnFinish;
            IsValid = true;
            FromId = string.Empty;
            Remark = string.Empty;

            Check();
        }

        private UserPointsTask()
        {
        }

        public bool Check()
        {
            var task = PointsTask;
            if (task == null)
            {
                throw new Exception("无此任务");
            }
            if (!task.IsEnable)
            {
                throw new Exception("任务已失效");
            }
            if (UserId == Guid.Empty)
            {
                throw new Exception("用户id无效");
            }
            return true;
        }

        public bool CheckTimes(int todayTimes, int totalTimes)
        {
            //历史完成总数
            if (PointsTask.MaxTimes > 0 && totalTimes >= PointsTask.MaxTimes)
            {
                throw new Exception("超出最大完成次数");
            }

            //今日完成总次数
            if (PointsTask.MaxTimesEveryDay > 0 && todayTimes >= PointsTask.MaxTimesEveryDay)
            {
                throw new Exception("超出每日最大完成次数");
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromId"></param>
        /// <param name="remark"></param>
        /// <returns>update fields</returns>
        public async Task<string[]> FinishAsync(DateTime finishTime, string fromId, string remark = "")
        {
            if (Status != UserPointsTaskStatus.UnFinish)
            {
                throw new Exception("任务已完成");
            }

            EndFinishTime = finishTime;
            Status = UserPointsTaskStatus.Finished;

            FromId = fromId ?? string.Empty;
            Remark += remark ?? string.Empty;

            //update fields
            return new string[] { nameof(EndFinishTime), nameof(Status), nameof(FromId), nameof(Remark) };
        }

        /// <summary>
        /// 领取积分
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string[]> GrantAsync(IMediator mediator)
        {
            if (Status != UserPointsTaskStatus.Finished)
            {
                throw new Exception("任务状态异常");
            }

            Status = UserPointsTaskStatus.PointsGranted;

            //发送完成任务, 有些任务需要自动发送积分
            await mediator.Publish(new GrantTaskDomainEvent(this));

            //update fields
            return new string[] { nameof(Status) };
        }
    }
}
