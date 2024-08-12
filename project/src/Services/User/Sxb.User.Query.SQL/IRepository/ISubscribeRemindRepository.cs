using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.User.Query.SQL.IRepository
{
    public interface ISubscribeRemindRepository
    {
        /// <summary>
        /// 判断是否订阅并且已关注公众号
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="subjectId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> ExistsAndSubscribeFwhAsync(string groupCode, Guid subjectId, Guid userId);
        Task<IEnumerable<Guid>> GetUserIdsAsync(string groupCode, Guid subjectId, int pageIndex, int pageSize);
    }
}