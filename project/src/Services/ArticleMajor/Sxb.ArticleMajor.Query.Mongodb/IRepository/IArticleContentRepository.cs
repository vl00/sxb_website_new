using MongoDB.Bson;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.ArticleMajor.Common.QueryDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb
{
    public interface IArticleContentRepository
    {
        Task AddAsync(IEnumerable<ArticleContent> entities);
        Task DeleteByArticleIds(string[] articleIds);
        Task<List<string>> GetArticleIds(int pageIndex, int pageSize);
        Task<List<string>> GetArticleIds(string lastArticleId, int pageSize);
        Task<ArticleContent> GetAsync(string articleId, int sort);
        Task<List<ArticleContent>> GetList(ObjectId? lastId, int pageIndex, int pageSize);
        Task UpdateContentAsync(IEnumerable<ArticleContent> entities);
    }
}