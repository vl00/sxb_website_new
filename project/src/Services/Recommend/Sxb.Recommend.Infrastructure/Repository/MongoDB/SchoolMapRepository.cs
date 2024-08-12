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
    public class SchoolMapRepository : Repository<SchoolMap, ObjectId>, ISchoolMapRepository
    {

        public SchoolMapRepository(IMongoClient mongoClient, IHostEnvironment hostEnvironment, ILogger<SchoolMapRepository> logger)
            : base(mongoClient, hostEnvironment, "SxbRecommend", logger)
        {

        }

        public async Task InsertManyAsync(IEnumerable<SchoolMap> documents)
        {
            using (var session = await _mongoClient.StartSessionAsync())
            {
                try
                {

                    session.WithTransaction((sessionHandle, token) =>
                    {
                        var filter = Builders<SchoolMap>.Filter.In(s => s.SIdP, documents.GroupBy(d => d.SIdP).Select(g => g.Key));
                        var deleteRes = MongoCollection.DeleteMany(session, filter);
                        MongoCollection.InsertMany(session, documents);
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

        public async Task InsertAsync(SchoolMap document)
        {
            using (var session = await _mongoClient.StartSessionAsync())
            {

                try
                {
                    //session.StartTransaction();
                    session.WithTransaction((sessionHandle, token) =>
                    {
                        var filter = Builders<SchoolMap>.Filter.And(
                            Builders<SchoolMap>.Filter.Eq(s => s.SIdP, document.SIdP)
                            , Builders<SchoolMap>.Filter.Eq(s => s.SIdS, document.SIdS)
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

        public async Task<IEnumerable<SchoolMap>> GetSchoolMaps(School school, int offset, int limit)
        {
            return await base.MongoCollection
                   .Find(s => s.SIdP == school.Id)
                   .SortByDescending(s => s.Score)
                   .Skip(offset)
                   .Limit(limit)
                   .ToListAsync();
        }

        public async Task<bool> HasSchoolMaps(School school)
        {
            return await base.MongoCollection
                .Find(s => s.SIdP == school.Id)
                .AnyAsync();
        }

        public async Task ClearAll()
        {
            await MongoCollection.DeleteManyAsync(s => 1 == 1);
        }

        public async Task<IEnumerable<SchoolMap>> GetSchoolMaps(School schoolS)
        {
            return await (await base.MongoCollection.FindAsync(s => s.SIdS == schoolS.Id)).ToListAsync();
        }

        public async Task<IEnumerable<SchoolMap>> GetSchoolMaps(Guid eid, int offset, int limit)
        {
            return await base.MongoCollection
                   .Find(s => s.SIdP == eid)
                   .SortByDescending(s => s.Score)
                   .Skip(offset)
                   .Limit(limit)
                   .ToListAsync();
        }
    }
}
