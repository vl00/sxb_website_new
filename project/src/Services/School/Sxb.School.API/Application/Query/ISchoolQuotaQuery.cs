using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface ISchoolQuotaQuery
    {
        /// <summary>
        /// 根据学部ID获取指标分配
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="year">年份</param>
        /// <param name="type">类型
        /// <para>
        /// 1.公办示范性普通高中指标计划分配
        /// 2.教育集团示范高中直接指标分配
        /// 3.指标到校名额分配
        /// </para>
        /// </param>
        /// <returns></returns>
        Task<IEnumerable<OnlineSchoolQuotaInfo>> GetByEID(Guid eid, int year = 0, int type = 0);
        /// <summary>
        /// 获取指定学部指标分配所有年份
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> GetYears(Guid eid);

        Task<bool> RemoveByEIDAsync(Guid eid);
        Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids);
        Task<bool> InsertAsync(OnlineSchoolQuotaInfo entity);
        Task<bool> SaveAsync(OnlineSchoolQuotaInfo entity);
        Task<OnlineSchoolQuotaInfo> GetAsync(Guid eid, int year = 0, int type = 0);
    }
}
