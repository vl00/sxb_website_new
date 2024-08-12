using MongoDB.Bson;
using Sxb.Domain;
using Sxb.Recommend.Domain.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Entity
{
    public class ArticleRedirectInside :  Entity<ObjectId>, IAggregateRoot
    {


        public ArticleRedirectInside(ObjectId id, Guid aidP, Guid aidS, DateTime createTime)
        {
            this.Id = id;
            this.AIdP = aidP;
            this.AIdS = aidS;
            this.CreateTime = createTime;
        }

        public ArticleRedirectInside(Guid aidP, Guid aidS, DateTime createTime)
        {
            this.Id = ObjectId.GenerateNewId();
            this.AIdP = aidP;
            this.AIdS = aidS;
            this.CreateTime = createTime;

            if (AIdP == Guid.Empty)
            {
                throw new Exception("源不存在");
            }
            if (AIdS == Guid.Empty)
            {
                throw new Exception("目标不存在");
            }
            if (AIdS == AIdP)
            {
                throw new Exception("源与目标不能相同");
            }
        }

        /// <summary>
        /// 主ID
        /// </summary>
        public Guid AIdP { get; set; }

        /// <summary>
        /// 次ID
        /// </summary>
        public Guid AIdS { get; set; }

        public DateTime CreateTime { get; set; }


        public void Create()
        {
            this.AddDomainEvent(new ArticleRedirectInsideCreateEvent(this));
        }
    }
}
