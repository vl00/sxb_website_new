using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Application.Queries
{
    public record School
    {
        public Guid ExtId { get; init; }

        public int Province { get; init; }

        public int City { get; init; }

        public int Area { get; init; }



    }
}
