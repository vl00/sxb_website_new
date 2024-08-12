using Sxb.School.Common.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface IAreaRecruitPlanQuery
    {
        /// <summary>
        /// 根据区域号码与学校类型获取
        /// </summary>
        /// <param name="areaCode">区域号码</param>
        /// <param name="schFType">学校类型</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        Task<IEnumerable<AreaRecruitPlanInfo>> GetByAreaCodeAndSchFType(string areaCode, string schFType, int year = 0);
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
