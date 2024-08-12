using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface ISchoolScoreQuery
    {
        Task<IEnumerable<SchoolExtensionScoreInfo>> GetScores(Guid eid);
        /// <summary>
        /// 获取当前类型的分数在区域超过的百分比
        /// </summary>
        /// <returns></returns>
        Task<double> GetSchoolRankingInCity(int cityCode, double score, string schFType);
        /// <summary>
        /// 获取学部树形分数
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<SchoolScoreTreeDTO> GetSchoolScoreTreeByEID(Guid eid);
    }
}
