using Sxb.School.Domain.AggregateModels.ViewOrder;
using System;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Queries.SchoolViewOrder
{
    public interface ISchoolViewOrderQueries
    {

        /// <summary>
        /// 是否为A级学部
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        Task<bool> IsALevelSchool(Guid eid);
        Task<bool> ExistsPermissionAsync(Guid userId,Guid eid);

        Task<ViewOrderStateSummary> GetViewOrderStateAsync(Guid orderId);

        /// <summary>
        /// 存在免单?
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> ExistsFreeOrder(Guid userId);

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<OrderDetail> GetOrderDetailAsync(Guid id);

        /// <summary>
        /// 获取学校信息
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        Task<SchoolInfo> GetSchoolInfo(Guid eid);


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserInfo> GetUserInfo(Guid id);

    }
}
