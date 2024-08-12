using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public interface IRecommendService
    {

         

        /// <summary>
        /// 获取推荐文章
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<ArticleMapValue>> GetRecommendArticles(Article article, int offset, int limit);

        /// <summary>
        /// 获取推荐文章
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetRecommendArticles(Guid aid, int offset, int limit);

        /// <summary>
        /// 获取推荐学校
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<SchoolMapValue>> GetRecommendSchools(School school, int offset, int limit);

        /// <summary>
        /// 获取推荐学校
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetRecommendSchools(Guid eid, int offset, int limit);




        /// <summary>
        /// 获取推荐学校
        /// </summary>
        /// <param name="filterDefinition">过滤定义</param>
        /// <returns></returns>
        Task<IEnumerable<SchoolScore>> GetRecommendSchools(SchoolFilterDefinition filterDefinition);



        /// <summary>
        /// 查询学校
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IEnumerable<School> QuerySchools(Func<School,bool> where);
        IEnumerable<Article> QueryArticles(Func<Article, bool> where);


        /// <summary>
        /// 为学校集中所有的生成学校匹配映射分数记录
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        Task InsertSchoolMaps();

    }
}
