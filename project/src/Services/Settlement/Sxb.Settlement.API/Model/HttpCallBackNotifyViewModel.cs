using Sxb.Settlement.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API
{

    public class SettlementStatusMessage
    {
        public string OrderNum { get; set; }
        
        public SettlementStatus Status { get; set; }

        public string FailReason { get; set; }

    }

    public class SettlementRefundSuccessMessage {

        public string OrderNum { get; set; }

        /// <summary>
        /// 退回金额
        /// </summary>
        public decimal RefundAmount { get; set; }
    }
}
