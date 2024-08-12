using MongoDB.Bson;
using MongoDB.Driver;
using ProductManagement.Framework.MongoDb;
using ProductManagement.Framework.MongoDb.UoW;
using Sxb.ArticleMajor.Common.MongoEntity;
using System.Collections.Generic;

namespace Sxb.ArticleMajor.Query.Mongodb
{
    public abstract class RepositoryBase<T>
    {
        protected readonly IMongoService _mongo;

        protected readonly IMongoDatabase _database;
        protected readonly IMongoCollection<T> _collection;

        protected readonly string _collectionName;

        public RepositoryBase(IMongoService mongo, string collectionName = null)
        {
            _mongo = mongo;

            //自定义
            //_database = _mongo.GetDatabase(Constant.DbName);

            //默认库
            _database = _mongo.ImongdDb;

            _collectionName = collectionName ?? typeof(T).Name;// nameof(T) == "T"
            _collection = _database.GetCollection<T>(_collectionName);
        }

        public List<BsonDocument> GetAggregate(List<string> pipelineJson, AggregateOptions options = null)
        {
            IList<IPipelineStageDefinition> stages = new List<IPipelineStageDefinition>();

            foreach (var line in pipelineJson)
            {
                stages.Add(new JsonPipelineStageDefinition<BsonDocument, BsonDocument>(line));
            }

            PipelineDefinition<BsonDocument, BsonDocument> pipeline = new PipelineStagePipelineDefinition<BsonDocument, BsonDocument>(stages);

            //查询结果
            return _database.GetCollection<BsonDocument>(_collectionName).Aggregate(pipeline, options).ToList();
        }
    }


}