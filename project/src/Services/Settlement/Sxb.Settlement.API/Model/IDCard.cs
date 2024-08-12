using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Model
{
    public class IDCard
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }

        public bool IsSign { get; set; } = false;

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
