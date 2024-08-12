using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.Repository.MongoDB
{
    public class MapFeatureRepository : Repository<MapFeature, ObjectId>, IMapFeatureRepository
    {
        IMemoryCache _cache;
        public MapFeatureRepository(IMongoClient mongoClient
            , IHostEnvironment environment
            , ILogger<MapFeatureRepository> logger
            , IMemoryCache cache)
            : base(mongoClient, environment, "SxbRecommend", logger)
        {
            _cache = cache;
        }

        public async Task<bool> HasMapFeature(int type)
        {
            var mapfeatures = await base.MongoCollection.FindAsync<MapFeature>(mp => mp.Type == type);
            return await mapfeatures.AnyAsync();


        }

        public override async Task<MapFeature> UpdateAsync(MapFeature entity, CancellationToken cancellationToken = default)
        {
            _cache.Remove($"mapFeature_{entity.Type}");

            return await base.UpdateAsync(entity, cancellationToken);
        }

        public async Task InsertManyAsync(IEnumerable<MapFeature> mapFeatures)
        {
            foreach (var mapFeature in mapFeatures)
            {
                _cache.Remove($"mapFeature_{mapFeature.Type}");
            }
            await base.MongoCollection.InsertManyAsync(mapFeatures);
        }

        public async Task<IEnumerable<MapFeature>> GetMapFeaturesAsync(int type)
        {
            return await _cache.GetOrCreateAsync($"mapFeature_{type}", async (cacheEntry) =>
            {
                var mapfeatures = await base.MongoCollection.Find(mf => mf.Type == type).SortBy(m => m.Id).ToListAsync();
                cacheEntry.SetSize(mapfeatures.Count);
                return mapfeatures;
            });

        }
    }
}
