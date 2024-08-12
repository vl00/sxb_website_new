using System;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Queries.DgAyOrder
{
    public interface IDgAyOrderQueries
    {

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<OrderDetail> GetOrderDetailAsync(Guid id);


        /// <summary>
        /// 获取订单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<DgAyOrderStateSummary> GetDgAyOrderStateAsync(Guid orderId);

        /// <summary>
        /// 存在免单?
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> ExistsFreeOrder(Guid userId);



    }
}
