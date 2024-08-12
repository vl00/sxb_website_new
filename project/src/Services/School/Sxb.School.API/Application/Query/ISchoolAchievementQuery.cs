using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface ISchoolAchievementQuery
    {
        Task<IEnumerable<OnlineSchoolAchievementInfo>> GetByEID(Guid eid, int year = 0);
        Task<IEnumerable<int>> GetYears(Guid eid);
        Task<IEnumerable<OnlineSchoolAchievementInfo>> GetAllAsync();
        Task<bool> InsertAsync(ExtensionAchievementInfo entity);
        Task<bool> SaveAsync(ExtensionAchievementInfo entity);
        Task<ExtensionAchievementInfo> GetAsync(Guid eid, int year = 0);
        Task<IEnumerable<ExtensionAchievementInfo>> ListByEIDAsync(Guid eid, int year = 0);
        Task<bool> RemoveByEIDAsync(Guid eid);
        Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids);
    }
}
