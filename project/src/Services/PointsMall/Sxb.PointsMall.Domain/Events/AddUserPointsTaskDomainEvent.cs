using Sxb.Domain;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.Domain.Events
{
    public class AddUserPointsTaskDomainEvent : IDomainEvent
    {
        /// <summary>
        /// 签到Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 签到用户
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 签到完成时间
        /// </summary>
        public DateTime FinishTime { get; set; }

        /// <summary>
        /// 签到天数
        /// </summary>
        public int SignInDays { get; set; }
    }
}
