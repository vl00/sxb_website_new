using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public interface ISchoolMapService
    {
        Task ClearAll();

        /// <summary>
        /// 生成某所学校的学校匹配映射分数记录
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        Task<IEnumerable<SchoolMap>> UpsertSchoolMaps(School school);

        /// <summary>
        /// 批量生成学校的学校匹配映射分数记录
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        Task UpsertSchoolMaps(IEnumerable<Guid> schoolIds);

    }
}
