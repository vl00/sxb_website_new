using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.IRepository
{
    public interface ISchoolAchievementRepository
    {
        /// <summary>
        /// 获取最近的年份
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<int> GetRecentYear(Guid eid);
        Task<int> GetMaxYearAsync(Guid eid);
        /// <summary>
        /// 根据学部ID获取升学成绩
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        Task<IEnumerable<OnlineSchoolAchievementInfo>> GetByEID(Guid eid, int year = 0);

        Task<IEnumerable<int>> GetYears(Guid eid);
        Task<IEnumerable<OnlineSchoolAchievementInfo>> GetAllAsync();
        Task<bool> InsertAsync(ExtensionAchievementInfo entity);
        Task<int> UpdateAsync(ExtensionAchievementInfo entity);
        Task<ExtensionAchievementInfo> GetAsync(Guid eid, int year = 0);
        Task<IEnumerable<ExtensionAchievementInfo>> ListByEIDAsync(Guid eid, int year = 0);
        /// <summary>
        /// 根据学部ID删除
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<bool> RemoveByEIDAsync(Guid eid);
        Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids);
    }
}
