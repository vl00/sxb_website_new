using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Model
{
    public record SchoolRankLog : DataLog
    {
        public List<int> Citys { get; init; }
    }
}
