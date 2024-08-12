using System;

namespace Sxb.School.API.Infrastructures.Services.Models
{
    public class AddPayOrderRequest
    {
        public AddPayOrderRequest(string userId, string orderId
            , string orderNo, decimal totalAmount
            , decimal payAmount, Orderbyproduct[] products
            , string openId, string attach
            , string remark
            ,string callBackLink
            ,DateTime orderExpireTime)
        {
            this.userId = userId;
            this.orderId = orderId;
            this.orderNo = orderNo;
            this.totalAmount=totalAmount;
            this.payAmount=payAmount;
            this.orderByProducts = products;
            this.openId = openId;
            this.attach = attach;
            this.remark = remark;
            this.callBackLink = callBackLink;
            this.orderExpireTime = orderExpireTime;
            
        }

        public string userId { get; set; }
        public string tradeNo { get; set; } = "";
        public string orderId { get; set; }
        public string orderNo { get; set; }
        public int orderType { get; set; }
        public int orderStatus { get; set; }
        public decimal totalAmount { get; set; }
        public decimal payAmount { get; set; }
        public int discountAmount { get; set; }
        public string remark { get; set; }
        public Orderbyproduct[] orderByProducts { get; set; }
        public string openId { get; set; }
        public string attach { get; set; }
        /// <summary>
        /// 4 -> 是学校
        /// </summary>
        public int system => 4;
        public int noNeedPay { get; set; } = 1;
        public int isRepay { get; set; } = 0;
        public string tOrderId { get; set; } = "";
        public int isWechatMiniProgram { get; set; } = 2;
        public string appId { get; set; } = "";
        public DateTime? orderExpireTime { get; set; }
        public int freightFee { get; set; }
        public string callBackLink { get; set; }

        public class Orderbyproduct
        {
            public decimal amount { get; set; }
            public int status { get; set; }
            public int productType { get; set; }
            public string productId { get; set; }
            public string remark { get; set; }
            public int buyNum { get; set; }
            public string advanceOrderId { get; set; }
            public string orderId { get; set; }
            public string orderDetailId { get; set; }
            public decimal price { get; set; }
        }

    }
}
