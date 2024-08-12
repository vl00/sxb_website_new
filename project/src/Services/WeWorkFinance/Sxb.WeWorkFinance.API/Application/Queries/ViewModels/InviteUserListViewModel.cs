using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Queries.ViewModels
{
    public class InviteUserListViewModel
    { 
        public string InviterUnionId { get; set; }
        public Guid UserId { get; set; }
        public int TotalPoint { get; set; }

        public DateTime LastEndTime { get; set; }
    }
}
