using Sxb.Domain;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.Domain.Events
{
    public class GrantTaskDomainEvent : IDomainEvent
    {
        public UserPointsTask UserPointsTask { get; set; }

        public GrantTaskDomainEvent(UserPointsTask userPointsTask)
        {
            UserPointsTask = userPointsTask;
        }
    }
}
