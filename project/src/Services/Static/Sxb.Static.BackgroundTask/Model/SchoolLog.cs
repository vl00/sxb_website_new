using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Model
{
    public record SchoolLog : DataLog
    {
        public int? Province { get; init; }
        public int? City { get; init; }
        public int? Area { get; init; }

    }
}
