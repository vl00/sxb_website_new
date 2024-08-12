using MongoDB.Bson;
using Sxb.Domain;
using Sxb.Recommend.Domain.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Entity
{
    public class ArticleRedirectFrequency : Entity<ObjectId>, IAggregateRoot
    {

        public ArticleRedirectFrequency(
            ObjectId id,
            Guid aidp,
            Guid aids,
            int openTimes
            )
        {
            this.Id = id;
            this.AIdP = aidp;
            this.AIdS = aids;
            this.OpenTimes = openTimes;

        }

        public ArticleRedirectFrequency(string fromCSVRow)
        {
            var fields = fromCSVRow.Split(",");
            this.Id = string.IsNullOrEmpty(fields[0]) ? ObjectId.Empty : ObjectId.Parse(fields[0]);
            this.AIdP = string.IsNullOrEmpty(fields[1]) ? Guid.Empty : Guid.Parse(fields[1]);
            this.AIdS = string.IsNullOrEmpty(fields[2]) ? Guid.Empty : Guid.Parse(fields[2]);
            this.OpenTimes = string.IsNullOrEmpty(fields[3]) ? 0 : int.Parse(fields[3]);
        }

        public Guid AIdP { get; private set; }

        public Guid AIdS { get; private set; }


        /// <summary>
        /// 打开次数
        /// </summary>
        public int OpenTimes { get; private set; }

        public string ToCSV()
        {
            return string.Format("{0},{1},{2},{3}", Id, AIdP, AIdS, OpenTimes);
        }
        public bool UpdateOpenTimes(int openTimes)
        {
            if (openTimes == this.OpenTimes)
            {
                return false;
            }
            else
            {
                this.OpenTimes = openTimes;
                this.AddDomainEvent(new ArticleOpenTimeIsChangeEvent(this));
                return true;
            }
        }

    }
}
