using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.IRepository
{
    public interface ISchoolFractionRepository
    {
        Task<IEnumerable<OnlineSchoolFractionInfo>> GetByEID(Guid eid, int year = 0);
        Task<IEnumerable<OnlineSchoolFractionInfo>> GetAllAsync();
        Task<IEnumerable<SchoolFractionInfo2>> Get2ByEID(Guid eid, int year = 0, int type = 0);
        Task<IEnumerable<SchoolFractionInfo2>> Get2AllAsync();
        /// <summary>
        /// 获取最近的年份
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<int> GetRecentYear(Guid eid);
        Task<int> Get2RecentYear(Guid eid);
        Task<int> GetMaxYearAsync(Guid eid);
        Task<IEnumerable<(int, int)>> Get2Years(Guid eid);
        Task<IEnumerable<int>> GetYears(Guid eid);
        Task<bool> InsertAsync(ExtensionFractionInfo entity);
        Task<int> UpdateAsync(ExtensionFractionInfo entity);
        Task<ExtensionFractionInfo> GetAsync(Guid eid, int year = 0, int type = 0);
        Task<IEnumerable<ExtensionFractionInfo>> ListByEIDAsync(Guid eid, int year = 0, ExtensionFractionType type = 0);

        Task<bool> RemoveByEIDAsync(Guid eid);
        Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids);
        Task<IEnumerable<(int, int)>> GetYearsAsync(Guid eid);
    }
}