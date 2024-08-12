using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Model
{
    public record CourseLog : DataLog
    {
        public int Type { get; init; }

    }
}
