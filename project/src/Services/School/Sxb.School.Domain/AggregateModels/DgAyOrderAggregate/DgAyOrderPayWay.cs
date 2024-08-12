using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.School.Domain.AggregateModels.DgAyOrderAggregate
{
    public enum DgAyOrderPayWay
    {

        /// <summary>
        /// 关注公众号免费订单(这么叫而已,实际上还要加客服的企业微信)
        /// </summary>
        SubscribeWPFree = 1,
        /// <summary>
        /// 微信支付
        /// </summary>
        WeChatPay = 2,
        /// <summary>
        /// 支付宝
        /// </summary>
        AliPay = 3

    }
}
