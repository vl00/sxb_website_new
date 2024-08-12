using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Queries.DgAyUserQaPaper
{
    public interface IDgAyUserQaPaperQueries
    {

        /// <summary>
        /// 获取分析结果中A级学校ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<Guid>> GetALevelEIds(Guid id);

        /// <summary>
        /// 获取分析结果中为开通查阅权限的A级学校ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<Guid>> GetALevelEIdsWithUnOpenPermission(Guid id);


        /// <summary>
        /// 获取 DgAyUserQaPaper
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DgAyUserQaPaper> GetDgAyUserQaPaper(Guid id);

    }
}
