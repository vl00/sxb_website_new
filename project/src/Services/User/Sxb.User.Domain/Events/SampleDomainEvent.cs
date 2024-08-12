using Sxb.Domain;
using Sxb.User.Domain.AggregatesModel.TalentAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.User.Domain.Events
{
    public class SampleDomainEvent : IDomainEvent
    {
        public Talent Talent { get; private set; }
        public SampleDomainEvent(Talent talent)
        {
            this.Talent = talent;
        }
    }
}
