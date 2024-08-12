using Sxb.Settlement.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Application.Queries
{
    public interface IUserIDCardQueries
    {
        Task<bool> ExistsOthersHasSignAsync(IDCard card);

        Task<bool> ExistsIDCard(IDCard card);


        /// <summary>
        /// 查询用户最新的第一个已经通过认证的身份证
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IDCard> GetFirstSignIdCard(Guid userId);

        /// <summary>
        /// 查询用户最新的第一个已经通过认证的身份证
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IDCard> GetFirstUnSignIdCard(Guid userId);


        /// <summary>
        /// 查询用户最新的身份证
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IDCard> GetFirstIdCard(Guid userId);

        /// <summary>
        /// 查询第一个也许已经验证过的IDCard。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IDCard> GetFirstMaybeSignIdCard(Guid userId);


        /// <summary>
        /// 查询第一个也许未验证过的IDCard。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IDCard> GetFirstMaybeUnSignIdCard(Guid userId);

        /// <summary>
        /// 获取认证次数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        Task<int> GetSignCount(Guid userId);
    }
}
