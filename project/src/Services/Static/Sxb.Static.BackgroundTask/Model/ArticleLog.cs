using MongoDB.Bson;
using Sxb.Static.BackgroundTask.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Model
{
    public record ArticleLog:DataLog
    {
        public List<DeployAreaInfo> DeployAreaInfo { get; init; }
    }
}
