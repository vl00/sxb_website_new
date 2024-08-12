using MongoDB.Driver;
using Sxb.Static.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Static.API.Infrastruture
{
    public class DataViewLogRepository : IDataViewLogRepository
    {
        private readonly IMongoClient _client;
        public DataViewLogRepository(IMongoClient mongoClient)
        {
            _client = mongoClient; 
        }

        public async Task AddAsync(DataViewLog dataViewLog)
        {
            var collection = _client.GetDatabase("SxbStatic").GetCollection<DataViewLog>("DataViewLog");
            await collection.InsertOneAsync(dataViewLog);
            
        }

        public async Task AddsAsync(IEnumerable<DataViewLog> dataViewLogs)
        {
            var collection = _client.GetDatabase("SxbStatic").GetCollection<DataViewLog>("DataViewLog");
            await collection.InsertManyAsync(dataViewLogs);
        }
    }
}
