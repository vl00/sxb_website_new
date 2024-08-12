using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.School.Domain.AggregateModels.ViewOrder
{
    public enum ViewOrderState
    {
       /// <summary>
       /// 待支付
       /// </summary>
        WaitPay = 1001,
        /// <summary>
        /// 等待支付结果返回
        /// </summary>
        WaitPayResult = 1002,
        /// <summary>
        /// 支付成功
        /// </summary>
        PaySuccess = 2001,
        /// <summary>
        /// 支付失败
        /// </summary>
        PayFail = 2002
    }
}
