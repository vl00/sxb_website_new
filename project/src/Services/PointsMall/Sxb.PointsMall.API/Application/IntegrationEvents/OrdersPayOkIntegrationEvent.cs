using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxb.PointsMall.API.Application.IntegrationEvents
{
    // 支付成功后
    public class OrdersPayOkIntegrationEvent
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// 大单ID
        /// </summary>
        public Guid AdvanceOrderId { get; set; }

        /// <summary>
        /// 大单编号
        /// </summary>
        public string AdvanceOrderNo { get; set; }

        /// <summary>
        /// 支付是否使用了积分
        /// </summary>
        public bool AdvanceOrderIsPointsPay { get; set; }
        
        public DateTime PaymentTime { get; set; }

        public List<Order> Orders { get; set; }

        public string GetProductNames(int? maxLength)
        {
            var names = string.Join(',',
                Orders?.SelectMany(s => s?.OrderDetails?.Select(s => s?.ProductName))
            );
            if (maxLength == null || names.Length < maxLength)
            {
                return names;
            }
            return names[..maxLength.Value];
        }
    }

    public class Order
    {

        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 小订单条项
        /// </summary>
        public List<OrderDetail> OrderDetails { get; set; }
    }


    public class OrderDetail
    {

        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; }
    }
}
