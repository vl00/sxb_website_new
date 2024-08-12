
using System;

namespace Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter.Model
{
    public class WalletRequest
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 冻结变动金额（正数）
        /// </summary>
        public decimal BlockedAmount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark{ get; set; }
    }
}