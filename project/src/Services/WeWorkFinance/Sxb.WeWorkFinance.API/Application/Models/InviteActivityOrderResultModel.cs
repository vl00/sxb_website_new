using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.Models
{
    public class InviteActivityOrderResultModel
    {
        public Guid OrderId { get; set; }
        public Guid OrderDetailId { get; set; }
        public int Count { get; set; }
        public string OrderNo { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public string StatusDesc { get; set; }
        public double ConsumedScores { get; set; }
    }
}
