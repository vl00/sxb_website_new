using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ProductManagement.Framework.MongoDb;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.ArticleMajor.Common.QueryDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb
{
    public class ArticleContentRepository : RepositoryBase<ArticleContent>, IArticleContentRepository
    {
        ProductManagement.Framework.MongoDb.UoW.IMongoDbUnitOfWork _unitOfWork;

        public ArticleContentRepository(IMongoService mongo, ProductManagement.Framework.MongoDb.UoW.IMongoDbUnitOfWork unitOfWork) : base(mongo)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<string>> GetArticleIds(int pageIndex, int pageSize)
        {
            var offset = (pageIndex - 1) * pageSize;
            var query = _collection.AsQueryable()
                .Select(s => s.ArticleId)
                .Skip(offset)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<List<string>> GetArticleIds(string lastArticleId, int pageSize)
        {
            if (pageSize < 100)
            {
                throw new Exception("分页太少, 可能页面都是相同ArticleId, 下一页失败");
            }

            var query = _collection.AsQueryable();

            if (!string.IsNullOrWhiteSpace(lastArticleId))
            {
                var last = (await _collection.FindAsync(s => s.ArticleId == lastArticleId)).FirstOrDefault();
                if (last != null)
                {
                    query = query.Where(s => s.Id > last.Id);
                }
                else
                {
                    return new List<string>();
                }
            }

            var entities = await query.Select(s => s.ArticleId).Take(pageSize).ToListAsync();

            var _lastArticleId = entities.Count != 0 ? entities?[^1] : string.Empty;
            //连续查到相同的数据, 一对多, 可能数据相同
            if (entities.Count != pageSize && lastArticleId == _lastArticleId)
            {
                return new List<string>();
            }
            return entities;
        }


        public async Task<List<ArticleContent>> GetList(ObjectId? lastId, int pageIndex, int pageSize)
        {
            if (pageIndex != 1 && lastId == null)
            {
                return new List<ArticleContent>();
            }

            var query = _collection.WithReadPreference(ReadPreference.SecondaryPreferred).AsQueryable();

            if (lastId != null)
            {
                query = query.Where(s => s.Id > lastId.Value);
            }

            return await query.Take(pageSize).ToListAsync();
        }

        public async Task DeleteByArticleIds(string[] articleIds)
        {
            await _collection.DeleteManyAsync(s => articleIds.Contains(s.ArticleId));
        }

        public async Task<ArticleContent> GetAsync(string articleId, int sort)
        {
            return (await _collection
                .FindAsync(s => s.ArticleId == articleId && s.Sort == sort)).FirstOrDefault();
        }

        public async Task AddAsync(IEnumerable<ArticleContent> entities)
        {
            if (_unitOfWork.SessionHandle != null)
            {
                await _collection.InsertManyAsync(_unitOfWork.SessionHandle, entities);
            }
            else
            {
                await _collection.InsertManyAsync(entities);
            }
        }

        public async Task UpdateContentAsync(IEnumerable<ArticleContent> entities)
        {
            if (entities == null || !entities.Any())
            {
                return;
            }

            var models = new List<WriteModel<ArticleContent>>(entities.Count());
            foreach (var item in entities)
            {
                FilterDefinition<ArticleContent> p1 = (Expression<Func<ArticleContent, bool>>)(s => s.Id == item.Id);
                UpdateDefinition<ArticleContent> p2 = Builders<ArticleContent>.Update.Set(u => u.Content, item.Content);
                models.Add(new UpdateOneModel<ArticleContent>(p1, p2));
            }
            await _collection.BulkWriteAsync(models);
        }
    }
}
