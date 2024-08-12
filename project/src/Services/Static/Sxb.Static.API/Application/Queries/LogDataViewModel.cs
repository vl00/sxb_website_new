using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Static.API.Application.Queries
{
    public class LogData
    {

        public Guid DataId { get; set; }

        public long? UV { get; set; }

    }
}
