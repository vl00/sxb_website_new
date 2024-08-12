using Sxb.User.Common.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.User.Query.SQL.IRepository
{
    public interface ITalentRepository
    {
        Task<TalentDTO> GetByUserID(Guid userID);
        Task<IEnumerable<TalentDTO>> ListByUserIDs(IEnumerable<Guid> userIDs);
        /// <summary>
        /// 获取学部付费达人用户ID
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<Guid> GetExtensionTalentUserID(Guid eid);
    }
}
