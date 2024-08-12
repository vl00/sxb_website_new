using Sxb.School.Common.OtherAPIClient.User.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.User
{
    public interface IUserAPIClient
    {
        /// <summary>
        /// 获取达人详情
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<TalentInfo> GetTalentDetail(Guid userID);
        /// <summary>
        /// 检查是否收藏
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckIsCollected(Guid dataID, Guid userID);

        /// <summary>
        /// 查询订阅用户
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="subjectId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetSubscribeRemindsUserIdsAsync(string groupCode, Guid subjectId, int pageIndex, int pageSize);
        Task<bool> CheckIsSubscribe(string groupCode, Guid subjectId, Guid userId);
    }
}
