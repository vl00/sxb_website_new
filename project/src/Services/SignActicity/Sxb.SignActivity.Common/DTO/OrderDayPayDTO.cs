using System;

namespace Sxb.SignActivity.Common.DTO
{
    public class OrderDayPayDTO
    {

        public Guid UserId { get; set; }


        /// <summary>
        /// 订单支付总额, 不含运费
        /// </summary>
        public decimal TotalPayment { get; set; }
        /// <summary>
        /// 订单退款总额, 可能含运费
        /// </summary>
        public decimal TotalRefund { get; set; }
        /// <summary>
        /// 订单退运费总额
        /// </summary>
        public decimal TotalRefundFreight { get; set; }
        
        public decimal RealPayment => TotalPayment - (TotalRefund - TotalRefundFreight);
    }
}