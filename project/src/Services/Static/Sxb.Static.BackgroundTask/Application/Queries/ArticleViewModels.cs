using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Application.Queries
{
    public record Article
    {

        public Guid Id { get; init; }

        public List<DeployAreaInfo> DeployAreaInfo { get; init; }
    }

    public record DeployAreaInfo
    {

        public int? Province { get; init; }

        public int? City { get; init; }

        public int? Area { get; init; }

    }
}
