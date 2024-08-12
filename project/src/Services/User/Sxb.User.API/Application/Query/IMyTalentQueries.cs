using Sxb.User.API.Application.Query.ViewModels;
using Sxb.User.Common.Entity;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sxb.User.Common.DTO;

namespace Sxb.User.API.Application.Query
{
    public interface IMyTalentQueries
    {
        MyTalentViewModel GetTalent(Guid userId);

        /// <summary>
        /// 根据UserInfo.ID获取达人信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<TalentDTO> GetByUserID(Guid userID);
        Task<IEnumerable<TalentDTO>> ListByUserIDs(IEnumerable<Guid> userIDs);
        Task<Guid> GetExtensionTalentUserID(Guid eid);
    }
}
