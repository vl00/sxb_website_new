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
    public class SchoolRedirectFrequencyRepository : Repository<SchoolRedirectFrequency, ObjectId>, ISchoolRedirectFrequencyRepository
    {
        public SchoolRedirectFrequencyRepository(IMongoClient mongoClient
            , IHostEnvironment environment
            , ILogger<SchoolRedirectFrequencyRepository> logger)
             : base(mongoClient, environment, "SxbRecommend", logger)
        {

        }
        public async Task<SchoolRedirectFrequency> QueryFrequencyAsync(Guid sidp, Guid sids)
        {
           var finds = await MongoCollection.FindAsync( s => s.SIdP == sidp && s.SIdS == sids);
            return finds.FirstOrDefault();
        }
        public async Task<IEnumerable<SchoolRedirectFrequency>> QueryFrequenciesAsync(int offset,int limit)
        {
            var db = this._mongoClient.GetDatabase(_dataBaseName);
            var collection = db.GetCollection<BsonDocument>($"{nameof(SchoolRedirectInside)}_{_environment.EnvironmentName}");
           var results =  collection.Aggregate()
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
                )
                .Skip(offset)
                .Limit(limit);
         return   (await results.ToListAsync()).Select(document =>
            {
                var SIdP = document.GetValue("SIdP", default(BsonBinaryData));
                var SIdS = document.GetValue("SIdS", default(BsonBinaryData));
                var Count = document.GetValue("Count",0);
                return new SchoolRedirectFrequency(ObjectId.GenerateNewId(), SIdP.AsGuid, SIdS.AsGuid, Count.AsInt32);
            });
        }


        public async Task<IEnumerable<SchoolRedirectFrequency>> QueryFrequenciesAsync(Guid sidp)
        {
            var finds = await MongoCollection.FindAsync(s => s.SIdP == sidp);
            return await finds.ToListAsync();
        }
    }
}
