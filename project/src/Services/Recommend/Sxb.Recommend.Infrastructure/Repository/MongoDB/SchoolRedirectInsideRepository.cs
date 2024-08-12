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
    public class SchoolRedirectInsideRepository : Repository<SchoolRedirectInside, ObjectId>, ISchoolRedirectInsideRepository
    {
        public SchoolRedirectInsideRepository(IMongoClient mongoClient
            , IHostEnvironment environment
            , ILogger<SchoolRedirectInsideRepository> logger)
             : base(mongoClient, environment, "SxbRecommend", logger)
        {

        }

        public async Task<List<SchoolRedirectInside>> ListAsync(System.Linq.Expressions.Expression<Func<SchoolRedirectInside, bool>> whereLambda = null)
        {
            return MongoCollection.AsQueryable().Where(whereLambda).OrderByDescending(s => s.CreateTime).ToList();
        }

        public async Task<SchoolRedirectFrequency> QueryFrequencyAsync(Guid sidp,Guid sids)
        {
            var db = this._mongoClient.GetDatabase(_dataBaseName);
            var collection = db.GetCollection<BsonDocument>($"{nameof(SchoolRedirectInside)}_{_environment.EnvironmentName}");
            var results = collection.Aggregate()
                 .Match(
                    new BsonDocument("$and",
                            new BsonArray
                            {
                                new BsonDocument("SIdP",BsonBinaryData.Create(sidp)),
                                new BsonDocument("SIdS",BsonBinaryData.Create(sids))
                            }
                    )
                 )
                 .Group(new BsonDocument{
                   { "_id",
                        new BsonDocument
                                {
                                    { "SIdp", "$SIdP" },
                                    { "SIdS", "$SIdS" }
                                }
                    },
                    { "Count",
                         new BsonDocument("$sum", 1)
                    }
                  }
                 )
                 .Project(
                     new BsonDocument
                         {
                            { "SIdP", "$_id.SIdp" },
                            { "SIdS", "$_id.SIdS" },
                            { "Count", "$Count" }
                         }
                 )
                 .Sort(new BsonDocument
                         {
                            { "_id", 1 },
                            { "Count", 1 }
                         }
                 );
            var document = await results.FirstOrDefaultAsync();
            var SIdP = document.GetValue("SIdP", default(BsonBinaryData));
            var SIdS = document.GetValue("SIdS", default(BsonBinaryData));
            var Count = document.GetValue("Count", 0);
            return new SchoolRedirectFrequency(ObjectId.GenerateNewId(), SIdP.AsGuid, SIdS.AsGuid, Count.AsInt32);
        }


    }
}
