using Sxb.School.Common.Entity;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sxb.School.API.Application.Query
{
    public interface IExtensionFieldQuery
    {
        /// <summary>
        /// 获取年份
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        Task<IEnumerable<YearExtFieldInfo>> ListYearsAsync(Guid eid, string field = default);
        /// <summary>
        /// 获取线上年份
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        Task<IEnumerable<OnlineYearExtFieldInfo>> ListOnlineYearsAsync(Guid eid, string field = default);

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="year">年份</param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        Task<IEnumerable<SchoolYearFieldContentInfo>> ListContentAsync(Guid eid, int year = default, string field = default);
        /// <summary>
        /// 获取在线内容
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="year">年份</param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        Task<IEnumerable<SchoolYearFieldContentInfo>> ListOnlineContentAsync(Guid eid, int year = default, string field = default);
    }
}
