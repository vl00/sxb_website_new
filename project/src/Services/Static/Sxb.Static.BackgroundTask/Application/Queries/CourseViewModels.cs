using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Application.Queries
{
    public record Course
    {
        public Guid Id { get; init; }

        public int Type { get; init; }
    }
}
