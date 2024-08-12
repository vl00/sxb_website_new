using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter.Model
{
    public class WalletResponse
    {
        /// <summary>
        /// 冻结ID
        /// </summary>
        public string FreezeMoneyInLogId { get; set; }

        /// <summary>
        /// 入账结果
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorDesc { get; set; }

    }
}
