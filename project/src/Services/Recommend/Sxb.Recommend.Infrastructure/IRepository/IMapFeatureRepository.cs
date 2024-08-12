using MongoDB.Bson;
using Sxb.Infrastructure.Core;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.IRepository
{
    public  interface IMapFeatureRepository : IRepository<MapFeature,ObjectId>
    {
       Task<IEnumerable<MapFeature>> GetMapFeaturesAsync(int type);

        Task InsertManyAsync(IEnumerable<MapFeature> mapFeatures);

        Task<bool> HasMapFeature(int type);

    }
}
