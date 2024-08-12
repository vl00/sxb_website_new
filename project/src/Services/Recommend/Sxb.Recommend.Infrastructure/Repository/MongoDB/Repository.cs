using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Sxb.Domain;
using Sxb.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.Repository.MongoDB
{
    public abstract class Repository<TEntity, TKey> : IRepository<TEntity, TKey>, IMongoRepository<TEntity> where TEntity : Entity<TKey>, IAggregateRoot
    {
        protected string _dataBaseName;
        protected string _collectionName;
        protected IMongoClient _mongoClient;
        protected ILogger _logger;
        protected IHostEnvironment _environment;



        protected IMongoCollection<TEntity> MongoCollection
        {
            get
            {
                var db = _mongoClient.GetDatabase(_dataBaseName);
                var colllection = db.GetCollection<TEntity>(_collectionName);
                return colllection;
            }
        }
        public Repository(IMongoClient mongoClient, IHostEnvironment environment, string database, ILogger logger)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _environment = environment;
            _dataBaseName = database;
            _collectionName = $"{typeof(TEntity).Name}_{_environment.EnvironmentName}";


        }
        public IUnitOfWork UnitOfWork => throw new NotImplementedException();


        public void InitCollection()
        {
            var db = _mongoClient.GetDatabase(_dataBaseName);
            var filter = new BsonDocument("name",_collectionName);
            var options = new ListCollectionNamesOptions { Filter = filter };
            if (!db.ListCollectionNames(options).Any())
            {
                db.CreateCollection(_collectionName);
            }
        }


        public TEntity Add(TEntity entity)
        {
            MongoCollection.InsertOne(entity);
            return entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await MongoCollection.InsertOneAsync(entity);
            return entity;
        }

        public void AddRange(List<TEntity> entities)
        {
            MongoCollection.InsertMany(entities);
        }

        public bool Delete(TKey id)
        {
            var res = MongoCollection.DeleteOne(t => t.Id.Equals(id));
            return res.DeletedCount > 0;
        }

        public async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var res = await MongoCollection.DeleteOneAsync(t => t.Id.Equals(id));
            return res.DeletedCount > 0;
        }

        public TEntity FirstOrDefault(System.Linq.Expressions.Expression<Func<TEntity, bool>> whereLambda = null)
        {
            return MongoCollection.Find(whereLambda).FirstOrDefault();
        }

        public TEntity Get(TKey id)
        {
            return MongoCollection.Find(t => t.Id.Equals(id)).FirstOrDefault();
        }

        public System.Linq.IQueryable<TEntity> GetAllIQueryable(System.Linq.Expressions.Expression<Func<TEntity, bool>> whereLambda = null)
        {

            throw new NotImplementedException();
        }

        public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await (await MongoCollection.FindAsync(t => t.Id.Equals(id))).FirstOrDefaultAsync();
        }

        public int GetCount(System.Linq.Expressions.Expression<Func<TEntity, bool>> whereLambd = null)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Entity entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAsync(Entity entity)
        {
            throw new NotImplementedException();
        }

        public TEntity Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var delFlag = await this.DeleteAsync(entity.Id);
            if (delFlag)
            {
                return await this.AddAsync(entity);
            }
            return null;

        }

        public void UpdateRange(List<TEntity> entities)
        {
            throw new NotImplementedException();
        }

    }
}
