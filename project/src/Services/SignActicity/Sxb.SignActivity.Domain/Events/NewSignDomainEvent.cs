using Sxb.Domain;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.SignActivity.Domain.Events
{
    public class NewSignDomainEvent : IDomainEvent
    {
        public SignIn Sign { get; private set; }
        public NewSignDomainEvent(SignIn sign)
        {
            this.Sign = sign;
        }
    }
}
