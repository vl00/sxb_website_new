using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Queries.ViewModels
{
    public class InviteUserPointViewModel
    {
        public string UnionId { get; set; }
        public int AddPoint { get; set; }
        public int InvalidPoint { get; set; }
    }
}
