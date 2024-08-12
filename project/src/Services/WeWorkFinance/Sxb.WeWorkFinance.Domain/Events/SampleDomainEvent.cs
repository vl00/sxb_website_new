using Sxb.Domain;
using Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Domain.Events
{
    public class SampleDomainEvent : IDomainEvent
    {
        public GroupUser User { get; private set; }
        public SampleDomainEvent(GroupUser user)
        {
            this.User = user;
        }
    }
}
