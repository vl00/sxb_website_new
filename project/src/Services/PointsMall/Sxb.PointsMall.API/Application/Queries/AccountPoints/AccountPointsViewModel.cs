using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Queries.AccountPoints
{

    public record AccountPoints
    {

        public Guid UserId { get; set; }

        public long Points { get; set; }

        public long FreezePoints { get; set; }
    }
}
