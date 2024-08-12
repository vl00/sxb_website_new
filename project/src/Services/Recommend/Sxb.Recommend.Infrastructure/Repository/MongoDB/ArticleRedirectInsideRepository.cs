using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.Repository.MongoDB
{
    public class ArticleRedirectInsideRepository : Repository<ArticleRedirectInside, ObjectId>, IArticleRedirectInsideRepository
    {
        public ArticleRedirectInsideRepository(IMongoClient mongoClient
            , IHostEnvironment environment
            , ILogger<SchoolRedirectInsideRepository> logger)
             : base(mongoClient, environment, "SxbRecommend", logger)
        {

        }

        public async Task<ArticleRedirectFrequencyValue> StaticFrequencyAsync(Guid aidp, Guid aids)
        {
            var db = this._mongoClient.GetDatabase(_dataBaseName);
            var collection = db.GetCollection<BsonDocument>($"{nameof(ArticleRedirectInside)}_{_environment.EnvironmentName}");
            var results = collection.Aggregate()
                 .Match(
                    new BsonDocument("$and",
                            new BsonArray
                            {
                                new BsonDocument("AIdP",BsonBinaryData.Create(aidp)),
                                new BsonDocument("AIdS",BsonBinaryData.Create(aids))
                            }
                    )
                 )
                 .Group(new BsonDocument{
                   { "_id",
                        new BsonDocument
                                {
                                    { "AIdP", "$AIdP" },
                                    { "AIdS", "$AIdS" }
                                }
                    },
                    { "OpenTimes",
                         new BsonDocument("$sum", 1)
                    }
                  }
                 )
                 .Project(
                     new BsonDocument
                         {
                            { "AIdP", "$_id.AIdP" },
                            { "AIdS", "$_id.AIdS" },
                            { "OpenTimes", "$OpenTimes" }
                         }
                 )
                 .Sort(new BsonDocument
                         {
                            { "_id", 1 },
                            { "OpenTimes", 1 }
                         }
                 );
            var document = await results.FirstOrDefaultAsync();
            var AIdP = document.GetValue("AIdP", default(BsonBinaryData));
            var AIdS = document.GetValue("AIdS", default(BsonBinaryData));
            var OpenTimes = document.GetValue("OpenTimes", 0);
            return new ArticleRedirectFrequencyValue() {  AIdP = AIdP.AsGuid,AIdS = AIdS.AsGuid, OpenTimes = OpenTimes.AsInt32 };
        }
    }
}
