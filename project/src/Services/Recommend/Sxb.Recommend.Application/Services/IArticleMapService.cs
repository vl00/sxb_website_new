using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public interface IArticleMapService
    {

        Task ClearAll();

        /// <summary>
        /// 更新某篇文章的文章映射分数记录
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        Task<IEnumerable<ArticleMap>> UpsertArticelMaps(Article article);

        /// <summary>
        /// 批量生成文章的文章映射分数记录
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        Task UpsertArticelMaps(IEnumerable<Guid> articleIds);
    }
}
