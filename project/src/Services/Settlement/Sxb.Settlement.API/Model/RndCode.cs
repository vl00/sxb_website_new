using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Model
{
    public class RndCode
    {
        public string Mobile { get; set; }
        public string Code { get; set; }
        public string CodeType { get; set; }
        public DateTime CodeTime { get; set; }
    }
}
