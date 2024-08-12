using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Entity
{
    public class ArticleMap : Entity<ObjectId>, IAggregateRoot
    {
        public ArticleMap(Guid aIdP, Guid aIdS, double score, string remark)
        {
            this.Id = ObjectId.GenerateNewId();
            this.AIdP = aIdP;
            this.AIdS = aIdS;
            this.Score = score;
            this.ModifiTime = DateTime.Now;
            this.Remark = remark;
        }

        /// <summary>
        /// 主文章ID
        /// </summary>
        public Guid AIdP { get; set; }


        /// <summary>
        /// 次文章ID
        /// </summary>
        public Guid AIdS { get; set; }


        public double Score { get; set; }

        public DateTime ModifiTime { get; set; }

        public string Remark { get; set; }





    }
}
