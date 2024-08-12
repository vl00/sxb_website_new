using System;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Queries.DgAyAddCustomerUser
{
    public interface IDgAyAddCustomerUserQueries
    {

        /// <summary>
        /// 是否添加
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> GetStatus(Guid userId);
    }
}
