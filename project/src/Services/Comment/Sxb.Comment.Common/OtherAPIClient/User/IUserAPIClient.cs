using Sxb.Comment.Common.OtherAPIClient.User.Model.Entity;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sxb.Comment.Common.OtherAPIClient.User
{
    public interface IUserAPIClient
    {
        /// <summary>
        /// 获取达人详情
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<TalentInfo> GetTalentDetail(Guid userID);
        Task<IEnumerable<TalentInfo>> ListTalentDetails(IEnumerable<Guid> userIDs);
    }
}
