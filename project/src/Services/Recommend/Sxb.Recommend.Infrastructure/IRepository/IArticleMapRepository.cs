using MongoDB.Bson;
using Sxb.Infrastructure.Core;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.IRepository
{
    public interface IArticleMapRepository : IRepository<ArticleMap,ObjectId>
    {
        Task<bool> HasArticleMaps(Article school);
        Task<IEnumerable<ArticleMap>> GetArticleMaps(Article Article,int offset,int limit);
        Task<IEnumerable<ArticleMap>> GetArticleMaps(Guid aid, int offset, int limit);
        Task InsertManyAsync(IEnumerable<ArticleMap> documents);

        Task InsertAsync(ArticleMap document);
        Task ClearAll();
        Task<IEnumerable<ArticleMap>> GetArticleMaps(Article ArticleS);
    }
}
