using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Queries.ViewModels
{
    public class UserInfoViewModel
    {
        public Guid UserId { get; set; }

        public string NickName { get; set; }

        public string UnionId { get; set; }

        public string OpenId { get; set; }
    }
}
