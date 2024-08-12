using MongoDB.Bson;
using Sxb.Domain;
using Sxb.Recommend.Domain.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Entity
{
    public class SchoolRedirectFrequency : Entity<ObjectId>, IAggregateRoot
    {

        public SchoolRedirectFrequency(
            ObjectId id,
            Guid sidp,
            Guid sids,
            int openTime
            )
        {
            this.Id = id;
            this.SIdP = sidp;
            this.SIdS = sids;
            this.OpenTime = openTime;

        }

        public SchoolRedirectFrequency(string fromCSVRow)
        {
            var fields = fromCSVRow.Split(",");
            this.Id = string.IsNullOrEmpty(fields[0]) ? ObjectId.Empty : ObjectId.Parse(fields[0]);
            this.SIdP = string.IsNullOrEmpty(fields[1]) ? Guid.Empty : Guid.Parse(fields[1]);
            this.SIdS = string.IsNullOrEmpty(fields[2]) ? Guid.Empty : Guid.Parse(fields[2]);
            this.OpenTime = string.IsNullOrEmpty(fields[3]) ? 0 : int.Parse(fields[3]);
        }

        public Guid SIdP { get; private set; }

        public Guid SIdS { get; private set; }

        public int OpenTime { get; private set; }

        public string ToCSV()
        {
            return string.Format("{0},{1},{2},{3}",Id, SIdP, SIdS, OpenTime);
        }
        public void OpenTimeIsChange()
        {
            this.AddDomainEvent(new SchoolOpenTimeIsChangeEvent(this));
        }

    }
}
