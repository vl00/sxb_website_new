using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.Repository.MongoDB
{
    public class ArticleRedirectFrequencyRepository : Repository<ArticleRedirectFrequency, ObjectId>, IArticleRedirectFrequencyRepository
    {
        public ArticleRedirectFrequencyRepository(IMongoClient mongoClient
                , IHostEnvironment environment
                , ILogger<ArticleRedirectFrequencyRepository> logger)
                : base(mongoClient, environment, "SxbRecommend", logger)
        {

        }

        public async Task<ArticleRedirectFrequency> GetOrCreate(Guid aidp, Guid aids)
        {
            var find = (await MongoCollection.FindAsync(s => s.AIdP == aidp && s.AIdS == aids)).FirstOrDefault();
            if (find == null)
            {
                return new ArticleRedirectFrequency(ObjectId.GenerateNewId(), aidp, aids, 0);
            }
            else {
                return find;
            }
        }

        public async Task<IEnumerable<ArticleRedirectFrequency>> QueryFrequenciesAsync(Guid aidp)
        {
            var finds = await MongoCollection.FindAsync(s => s.AIdP == aidp);
            return await finds.ToListAsync();
        }

        public async Task UpsertFrequency(ArticleRedirectFrequency redirectFrequency)
        {
            using (var session = await _mongoClient.StartSessionAsync())
            {
                try
                {

                    session.WithTransaction((sessionHandle, token) =>
                    {
                        var filter = Builders<ArticleRedirectFrequency>.Filter.And(
                             Builders<ArticleRedirectFrequency>.Filter.Eq(s=>s.AIdP,redirectFrequency.AIdP)
                             , Builders<ArticleRedirectFrequency>.Filter.Eq(s => s.AIdS, redirectFrequency.AIdS)
                            );
                        var deleteRes = MongoCollection.DeleteMany(session, filter);
                        MongoCollection.InsertOne(session, redirectFrequency);
                        return deleteRes;
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, null);
                }
            }

        }
    }
}
