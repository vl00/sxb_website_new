using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Queries.ViewModels
{
    public class InviterUserStatisticalViewModel
    {
        public int Total { get; set; }
        public int ValidTotal { get; set; }

        public List<InviterUserDataViewModel> List { get; set; }
    }
    public class InviterUserDataViewModel
    {
        public string UnionId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string InviteTypeDesc { get; set; }
        public int InviteType { get; set; }
    }
}
