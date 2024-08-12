using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Queries.ViewModels
{
    public class CustomerViewModel
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }

        public int? City { get; set; }
        public int? Type { get; set; }
    }
}
