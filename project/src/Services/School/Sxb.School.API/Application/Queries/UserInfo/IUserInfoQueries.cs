using System;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Queries.UserInfo
{
    public interface IUserInfoQueries
    {


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserInfo> GetUserInfo(Guid id);

        /// <summary>
        /// 获取关注状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> GetSubscribeStatus(Guid id);


        /// <summary>
        /// 获取OpenId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
         Task<string> GetOpenId(Guid id);

    }
}
