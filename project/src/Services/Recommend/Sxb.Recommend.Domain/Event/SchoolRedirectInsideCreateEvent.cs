using Sxb.Domain;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Event
{
    public class SchoolRedirectInsideCreateEvent : IDomainEvent
    {
        public SchoolRedirectInside SchoolRedirectInside  { get;private set; }
        public SchoolRedirectInsideCreateEvent(SchoolRedirectInside schoolRedirectInside)
        {
            SchoolRedirectInside = schoolRedirectInside;
        }
    }
}
