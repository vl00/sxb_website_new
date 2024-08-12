using MongoDB.Bson;
using Sxb.Domain;
using Sxb.Recommend.Domain.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Entity
{
    public class SchoolRedirectInside : RedirectInside
    {
        public SchoolRedirectInside(ObjectId id, Guid sidP, Guid sidS, DateTime createTime) : base(id, sidP, sidS, createTime)
        {
        }

        public SchoolRedirectInside(Guid sidP, Guid sidS, DateTime createTime) : base(sidP, sidS, createTime) { 
        
        }

        public void Create()
        {
            this.AddDomainEvent(new SchoolRedirectInsideCreateEvent(this));
        
        }
    }
}
