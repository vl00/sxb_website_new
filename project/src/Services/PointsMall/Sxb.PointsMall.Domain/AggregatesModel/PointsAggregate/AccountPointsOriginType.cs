using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate
{
   public enum AccountPointsOriginType
    {


        [Description("兑换")]
        Exchange  =1,


        [Description("日常任务")]
        DayTask = 2,


        [Description("运营任务")]
        OperationTask =3,


        [Description("活动")]
        Activity = 4,


        /// <summary>
        /// 下单
        /// </summary>
        [Description("下单")]
        Orders = 5,

        /// <summary>
        /// 订单失效
        /// </summary>
        [Description("订单失效")]
        OrderLoseEfficacy = 6,


    }
}
