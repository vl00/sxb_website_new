using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Static.API.Model
{
    public class DataViewLog
    {

        public ObjectId Id { get; set; }
        public Guid DataId { get; set; }
        public DataType DataType { get; set; }
        public Guid? UserId { get; set; }
        public Guid? DeviceId { get; set; }
        public int? Province { get; set; }
        public int? City { get; set; }
        public int? Area { get; set; }
        public DateTime CreateTime { get; set; }

    }
}
