using Sxb.School.Common.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.IRepository
{
    public interface IAreaRecruitPlanRepository
    {
        Task<IEnumerable<AreaRecruitPlanInfo>> GetByAreaCodeAndSchFType(string areaCode, string schFType, int year = 0);
        /// <summary>
        /// 获取最近的年份
        /// </summary>
        /// <returns></returns>
        int GetRecentYear(string areaCode, string schFType);
        Task<IEnumerable<int>> GetYears(string areaCode, string schFType);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="deleteParams">批量参数
        /// <para>
        /// 元素格式为 {AreaCode}{SchFType}{Year}
        /// </para>
        /// </param>
        /// <returns></returns>
        Task<int> RemoveByParamsAsync(IEnumerable<string> deleteParams);

        Task<bool> InsertAsync(AreaRecruitPlanInfo entity);
    }
}
