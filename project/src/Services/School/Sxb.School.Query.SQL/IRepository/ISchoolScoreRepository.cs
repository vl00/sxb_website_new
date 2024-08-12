using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.IRepository
{
    public interface ISchoolScoreRepository
    {
        /// <summary>
        /// 获取学部分数
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<IEnumerable<SchoolExtensionScoreInfo>> GetExtensionScores(Guid eid);
        /// <summary>
        /// 查询传入学校分数在指定城市的评分排名
        /// </summary>
        /// <returns></returns>
        Task<double> GetSchoolRankingInCity(int cityCode, double score, string schFType);
        /// <summary>
        /// 获取全部分数Index
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ScoreIndexInfo>> GetAllScoreIndexs();

        /// <summary>
        /// 根据年级查询分数最高的前n所学校的学部eid
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetEidsForSchoolScoreTopNByGrade(int top, int grade);


        /// <summary>
        /// 根据学部eid查询学校分数
        /// </summary>
        Task<IEnumerable<(Guid Eid, double Score)>> GetSchoolsScoreEids(IEnumerable<Guid> eids);
    }
}
