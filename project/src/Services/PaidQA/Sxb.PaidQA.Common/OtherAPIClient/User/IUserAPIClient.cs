using Sxb.PaidQA.Common.OtherAPIClient.User.Model.Entity;
using System;
using System.Threading.Tasks;

namespace Sxb.PaidQA.Common.OtherAPIClient.User
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
        /// 获取学部绑定的付费达人UserID 
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<Guid> GetExtensionTalentUserID(Guid eid);
    }
}
