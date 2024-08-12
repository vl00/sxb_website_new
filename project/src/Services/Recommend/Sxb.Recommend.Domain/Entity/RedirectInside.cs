using MongoDB.Bson;
using Sxb.Domain;
using Sxb.Recommend.Domain.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Entity
{
    public class RedirectInside : Entity<ObjectId>, IAggregateRoot
    {
        public RedirectInside(ObjectId id, Guid sidP, Guid sidS, DateTime createTime)
        {
            this.Id = id;
            this.SIdP = sidP;
            this.SIdS = sidS;
            this.CreateTime = createTime;
        }

        public RedirectInside(Guid sidP, Guid sidS, DateTime createTime)
        {
            this.Id = ObjectId.GenerateNewId();
            this.SIdP = sidP;
            this.SIdS = sidS;
            this.CreateTime = createTime;

            if (SIdP == Guid.Empty)
            {
                throw new Exception("源不存在");
            }
            if (SIdS == Guid.Empty)
            {
                throw new Exception("目标不存在");
            }
            if (SIdS == SIdP)
            {
                throw new Exception("源与目标不能相同");
            }
        }

        /// <summary>
        /// 主ID
        /// </summary>
        public Guid SIdP { get; set; }

        /// <summary>
        /// 次ID
        /// </summary>
        public Guid SIdS { get; set; }

        public DateTime CreateTime { get; set; }

    }
}
