using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.IRepository
{
    public interface ISchoolRecruitRepository
    {
        /// <summary>
        /// 根据学部ID获取招生信息
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<IEnumerable<OnlineSchoolRecruitInfo>> GetByEID(Guid eid, int year = 0, int type = 0);
        Task<IEnumerable<OnlineSchoolRecruitInfo>> ListByEIDsAsync(IEnumerable<Guid> eids, int year = 0, int type = 0);
        /// <summary>
        /// 获取最近的年份
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        int GetRecentYear(Guid eid);
        /// <summary>
        /// 获取招生日程
        /// </summary>
        /// <param name="recruitIDs">招生信息ID(复数)</param>
        /// <returns></returns>
        Task<IEnumerable<RecruitScheduleInfo>> GetRecruitScheduleByRecruitIDs(IEnumerable<Guid> recruitIDs);
        /// <summary>
        /// 获取招生日程
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="recruitTypes"></param>
        /// <param name="schFType"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        Task<IEnumerable<RecruitScheduleInfo>> GetRecruitSchedule(int cityCode, IEnumerable<int> recruitTypes, string schFType,int year, int? areaCode = null);

        Task<IEnumerable<int[]>> GetRecruitYears(Guid eid);
        Task<IEnumerable<int>> GetCostYears(Guid eid);
        Task<IEnumerable<OnlineSchoolRecruitInfo>> GetCostByYearAsync(Guid eid, int year);
        Task<bool> RemoveByEIDAsync(Guid eid);
        Task<bool> InsertAsync(OnlineSchoolRecruitInfo entity);
        Task<bool> UpdateAsync(OnlineSchoolRecruitInfo entity);
        Task<bool> RemoveByEIDsAsync(IEnumerable<Guid> eids);
        Task<int> InsertManyAsync(IEnumerable<OnlineSchoolRecruitInfo> entities);
        Task<int> RemoveRecruitSchedulesAsync(IEnumerable<string> deleteParams);
        Task<int> InsertRecruitScheduleAsync(RecruitScheduleInfo entity);
    }
}
