using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Entity
{
    public class SchoolMap : Entity<ObjectId>, IAggregateRoot
    {
        public SchoolMap(Guid sIdP, Guid sIdS, double score, string remark)
        {
            this.Id = ObjectId.GenerateNewId();
            this.SIdP = sIdP;
            this.SIdS = sIdS;
            this.Score = score;
            this.ModifiTime = DateTime.Now;
            this.Remark = remark;
        }

        public SchoolMap(string fromCSVRow)
        {
            var fields = fromCSVRow.Split(",");
            this.Id = string.IsNullOrEmpty(fields[0]) ? ObjectId.Empty : ObjectId.Parse(fields[0]);
            this.SIdP = string.IsNullOrEmpty(fields[1]) ? Guid.Empty : Guid.Parse(fields[1]);
            this.SIdS = string.IsNullOrEmpty(fields[2]) ? Guid.Empty : Guid.Parse(fields[2]);
            this.Score = string.IsNullOrEmpty(fields[3]) ? 0 : double.Parse(fields[3]);
            this.ModifiTime = string.IsNullOrEmpty(fields[4]) ? DateTime.Now : DateTime.Parse(fields[4]);
            this.Remark = fields[5];


        }
        public string ToCSV()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}", this.Id, SIdP, SIdS, Score, ModifiTime, Remark);

        }
        /// <summary>
        /// 主学部ID
        /// </summary>
        //[BsonGuidRepresentation(GuidRepresentation.Unspecified)]
        public Guid SIdP { get; set; }
        /// <summary>
        /// 次学部ID
        /// </summary>
        //[BsonGuidRepresentation(GuidRepresentation.Unspecified)]
        public Guid SIdS { get; set; }
        public double Score { get; set; }

        public DateTime ModifiTime { get; set; }

        public string Remark { get; set; }





    }
}
