using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.School.Domain.AggregateModels.ViewPermission
{
    public  class SchoolViewPermission: Entity<Guid>,IAggregateRoot
    {
        public Guid UserId { get; private set; }
        public Guid ExtId { get; private set; }
        public bool IsValid { get; private set; }
        public DateTime CreateTime { get; private set; }
        public DateTime UpdateTime { get; private set; }



        public static SchoolViewPermission NewDraft(Guid userId,Guid extId) {
            return new SchoolViewPermission(Guid.NewGuid(),userId,extId,true,DateTime.Now,DateTime.Now);
        }

        public SchoolViewPermission(Guid id, Guid userId,Guid extId,bool isValid,DateTime createTime,DateTime updateTime)
        {
            this.Id = id;
            this.UserId = userId;
            this.ExtId = extId;
            this.IsValid = isValid;
            this.CreateTime = createTime;
            this.UpdateTime = updateTime;

        }


    }
}
