using Sxb.SignActivity.Common.DTO;
using Sxb.SignActivity.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Query.SQL.IRepository
{
    public interface IOrderRepository
    {
        Task<Order> GetAsync(Guid orderId);
        Task<IEnumerable<Order>> GetListAsync(DateTime startTime, DateTime endTime, int? status = null);
        Task<IEnumerable<OrderDayPayDTO>> GetOrderDayPaysAsync(Guid? userId, DateTime startTime, DateTime endTime, int? status = null);

        /// <summary>
        /// 获取订单支付当日的[实际收货的订单金额]
        /// 实际收货订单金额 = 收货总支付 - 总退款
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<decimal> GetShippedTotalAmountAsync(Guid userId, DateTime signDate);

        /// <summary>
        /// 获取实际订单金额 = 总支付 - 总退款
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<decimal> GetTotalAmountAsync(Guid userId, DateTime startTime, DateTime endTime, int? status = null);


        /// <summary>
        /// 获取支付总额
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<decimal> GetTotalOrderPayAsync(Guid userId, DateTime startTime, DateTime endTime, int? status = null);


        /// <summary>
        /// 获取退款总额
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<decimal> GetTotalOrderRefundAsync(Guid userId, DateTime startTime, DateTime endTime);
    }
}