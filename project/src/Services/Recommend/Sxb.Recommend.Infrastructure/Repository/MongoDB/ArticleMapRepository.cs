using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.Repository.MongoDB
{
    public class ArticleMapRepository : Repository<ArticleMap, ObjectId>, IArticleMapRepository
    {

        public ArticleMapRepository(IMongoClient mongoClient, IHostEnvironment hostEnvironment, ILogger<ArticleMapRepository> logger)
            : base(mongoClient, hostEnvironment, "SxbRecommend", logger)
        {

        }

        public async Task InsertManyAsync(IEnumerable<ArticleMap> documents)
        {
            using (var session = await _mongoClient.StartSessionAsync())
            {
                try
                {
                    //session.StartTransaction();
                    session.WithTransaction((sessionHandle, token) =>
                    {
                        var filter = Builders<ArticleMap>.Filter.In(s => s.AIdP, documents.GroupBy(d => d.AIdP).Select(g => g.Key));
                        var deleteRes = MongoCollection.DeleteMany(sessionHandle, filter);
                        MongoCollection.InsertMany(sessionHandle, documents);
                        return deleteRes;
                    });
                }
                catch (Exception ex)
                {
                    //await session.AbortTransactionAsync();
                    _logger.LogError(ex, null);
                }
            }

        }

        public async Task InsertAsync(ArticleMap document)
        {
            using (var session = await _mongoClient.StartSessionAsync())
            {

                try
                {
                    //session.StartTransaction();
                    session.WithTransaction((sessionHandle, token) =>
                    {
                        var filter = Builders<ArticleMap>.Filter.And(
                            Builders<ArticleMap>.Filter.Eq(s => s.AIdP, document.AIdP)
                            , Builders<ArticleMap>.Filter.Eq(s => s.AIdS, document.AIdS)
                            );
                        var deleteRes = MongoCollection.DeleteMany(filter);
                        MongoCollection.InsertOne(document);
                        return deleteRes;
                    });

                    //await session.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    //await session.AbortTransactionAsync();
                    _logger.LogError(ex, null);
                }

            }


        }

        public async Task<IEnumerable<ArticleMap>> GetArticleMaps(Article Article, int offset, int limit)
        {
            return await base.MongoCollection
                   .Find(s => s.AIdP == Article.Id)
                   .SortByDescending(s => s.Score)
                   .Skip(offset)
                   .Limit(limit)
                   .ToListAsync();
        }

        public async Task<bool> HasArticleMaps(Article Article)
        {
            return await base.MongoCollection
                .Find(s => s.AIdP == Article.Id)
                .AnyAsync();
        }

        public async Task ClearAll()
        {
            await MongoCollection.DeleteManyAsync(s => 1 == 1);
        }

        public async Task<IEnumerable<ArticleMap>> GetArticleMaps(Article ArticleS)
        {
            return await (await base.MongoCollection.FindAsync(s => s.AIdS == ArticleS.Id)).ToListAsync();
        }

        public async Task<IEnumerable<ArticleMap>> GetArticleMaps(Guid aid, int offset, int limit)
        {
            return await base.MongoCollection
                   .Find(s => s.AIdP == aid)
                   .SortByDescending(s => s.Score)
                   .Skip(offset)
                   .Limit(limit)
                   .ToListAsync();
        }
    }
}
