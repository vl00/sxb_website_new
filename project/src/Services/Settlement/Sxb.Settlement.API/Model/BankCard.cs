using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Model
{
    public class BankCard
    {
        public string Number { get; set; }

        public string Mobile { get; set; }

        public string CheckCode { get; set; }

        public string AreaCode { get; set; }
    }
}
