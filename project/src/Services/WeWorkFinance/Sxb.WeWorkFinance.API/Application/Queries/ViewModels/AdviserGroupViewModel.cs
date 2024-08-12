using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Queries.ViewModels
{
    public class AdviserGroupViewModel 
    {
        public string UnionId { get; set; }
        public string GroupQrCodeUrl { get; set; }
        public string WorkGroup { get; set; }
        public int City { get; set; }
    }
}
