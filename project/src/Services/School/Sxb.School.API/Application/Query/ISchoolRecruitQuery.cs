using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface ISchoolRecruitQuery
    {
        /// <summary>
        /// 根据学部ID获取招生信息
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="year">年份</param>
        /// <param name="type">类型
        /// <para>
        /// 0.招生信息
        /// 1.本校招生信息
        /// 2.户籍生招生信息
        /// 3.非户籍生积分入学
        /// 4.分类招生简章
        /// 5.社会公开招生简章
        /// 6.自主招生
        /// 7.面向“5+2”区域招生
        /// 8.面向非“5+2”区域招生
        /// 9.项目班招生计划
        /// </para>
        /// </param>
        /// <returns></returns>
        Task<IEnumerable<OnlineSchoolRecruitInfo>> GetByEID(Guid eid, int year = 0, int type = 0);
        Task<IEnumerable<OnlineSchoolRecruitInfo>> ListByEIDsAsync(IEnumerable<Guid> eids, int year = 0, int type = 0);
        /// <summary>
        /// 获取招生日程
        /// </summary>
        /// <param name="recruitIDs"></param>
        /// <returns></returns>
        Task<IEnumerable<RecruitScheduleInfo>> GetRecruitSchedules(IEnumerable<Guid> recruitIDs);
        /// <summary>
        /// 获取招生日程
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="recruitTypes"></param>
        /// <param name="schFType"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        Task<IEnumerable<RecruitScheduleInfo>> GetRecruitSchedules(int cityCode, IEnumerable<int> recruitTypes, string schFType, int year, int? areaCode = null);
        Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> GetYears(Guid eid);
        /// <summary>
        /// 获取费用年份
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        Task<IEnumerable<int>> GetCostYears(Guid eid);
        Task<IEnumerable<OnlineSchoolRecruitInfo>> GetCostByYearAsync(Guid eid, int year);

        Task<bool> RemoveByEIDAsync(Guid eid);
        Task<bool> InsertAsync(OnlineSchoolRecruitInfo entity);
        Task<bool> SaveAsync(OnlineSchoolRecruitInfo entity);
        Task<int> InsertManyAsync(IEnumerable<OnlineSchoolRecruitInfo> entities);
        Task<bool> RemoveByEIDsAsync(IEnumerable<Guid> eids);
        /// <summary>
        /// 删除 招生日程
        /// </summary>
        /// <param name="deleteParams">元素格式为 {AreaCode}{SchFType}{RecruitType}{CityCode}</param>
        /// <returns></returns>
        Task<int> RemoveRecruitSchedulesAsync(IEnumerable<string> deleteParams);
        Task<int> InsertRecruitScheduleAsync(RecruitScheduleInfo entity);
    }
}
