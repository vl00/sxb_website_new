using MongoDB.Driver;
using Sxb.Infrastructure.Core;
using Sxb.Static.BackgroundTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Infrastucture
{

    public class StaticDbContext : IUnitOfWork
    {
        private static readonly object _dbSetsLock = new object();
        private string DB_Scheme;
        IDictionary<string, DbSet> _dbSets = new Dictionary<string, DbSet>();
        IMongoClient _mongoClient;
        public StaticDbContext(IMongoClient mongoClient,string dbname)
        {
            _mongoClient = mongoClient;
            DB_Scheme = dbname;
        }


        public DbSet<SchoolLog> SchoolLogs=> Set<SchoolLog>();
        public DbSet<ArticleLog> ArticleLogs => Set<ArticleLog>();

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            string entityName = typeof(TEntity).Name;
            lock (_dbSetsLock)
            {
                if (!_dbSets.ContainsKey(entityName))
                {
                    var collection = this._mongoClient.GetDatabase(DB_Scheme).GetCollection<TEntity>($"{entityName}s");
                    var dbSet = new DbSet<TEntity>(collection);
                    _dbSets.Add(entityName, dbSet);
                    return dbSet;
                }
                else
                {
                    return _dbSets[entityName] as DbSet<TEntity>;
                }
            }

        }

        public void Dispose()
        {
            _dbSets.Clear();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            using (var session = await _mongoClient.StartSessionAsync())
            {
                try
                {
                    return  session.WithTransaction((sessionHandle, token) =>
                    {
                        int result = 0;
                        foreach (var dbSet in _dbSets)
                        {
                            var commit = dbSet.Value.Commit(sessionHandle);
                            result += (int)commit;
                        }
                        return result;
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception("SaveChangesAsync Erro", ex);
                }
            }



        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await this.SaveChangesAsync(cancellationToken);
            return true;

        }
    }

    public class DbSet<T> : DbSet
    {
        private readonly object _commitLock = new object();
        public List<T> insertSets = new List<T>();
        public List<(Expression<Func<T, bool>>, UpdateDefinition<T>)> updateSets = new List<(Expression<Func<T, bool>>, UpdateDefinition<T>)>();
        public List<Expression<Func<T, bool>>> delSets = new List<Expression<Func<T, bool>>>();
        public IMongoCollection<T> Collection { get; init; }
        public DbSet(IMongoCollection<T> collection)
        {
            Collection = collection;
        }

        public void Add(T entity)
        {
            lock (_commitLock)
            {
                insertSets.Add(entity);
            }
        }
        public void AddRange(IEnumerable<T> entitys)
        {
            lock (_commitLock)
            {
                insertSets.AddRange(entitys);
            }

        }

        public void Update(Expression<Func<T, bool>> filter, UpdateDefinition<T> definition)
        {
            lock (_commitLock)
            {
                updateSets.Add((filter, definition));
            }


        }

        public void Delete(Expression<Func<T, bool>> filter)
        {
            lock (_commitLock)
            {
                delSets.Add(filter);
            }
        }

        public override long Commit(IClientSessionHandle sessionHandle = null)
        {
            lock (_commitLock)
            {
                long result = 0;
                Collection.InsertMany(sessionHandle, insertSets);
                result += insertSets.Count;
                foreach (var updateSet in updateSets)
                {
                    var updateResult = Collection.UpdateMany(sessionHandle, updateSet.Item1, updateSet.Item2);
                    result += updateResult.ModifiedCount;
                }
                foreach (var delset in delSets)
                {
                    var deleteResult = Collection.DeleteMany(sessionHandle, delset);
                    result += deleteResult.DeletedCount;
                }
                insertSets.Clear();
                updateSets.Clear();
                delSets.Clear();
                return result;
            }

        }

    }

    public abstract class DbSet
    {

        public abstract long Commit(IClientSessionHandle sessionHandle = null);
    }





    public class StaticDbContext<T> : IUnitOfWork
    {

        private readonly object _datalock = new object();
        private string DB_Scheme = "SxbStatic";
        private string Colletion = string.Format("{0}s", typeof(T).Name);
        private List<(T, OPType)> data = new List<(T, OPType)>();
        public IReadOnlyCollection<(T, OPType)> Data => data.AsReadOnly();
        IMongoClient _mongoClient;
        public StaticDbContext(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }
        public void Add(T entity)
        {
            lock (_datalock)
            {
                data.Add((entity, OPType.Insert));
            }

        }


        public void Dispose()
        {
            lock (_datalock)
            {
                data.Clear();
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int result;
            List<IGrouping<OPType, (T, OPType)>> groupDatas;
            lock (_datalock)
            {
                groupDatas = data.GroupBy(d => d.Item2).ToList();
                result = data.Count;
                this.data.Clear();
            }
            foreach (var groupData in groupDatas)
            {
                switch (groupData.Key)
                {
                    case OPType.Insert:
                        var collection = _mongoClient.GetDatabase(DB_Scheme).GetCollection<T>(Colletion);
                        await collection.InsertManyAsync(groupData.Select(s => s.Item1));
                        break;
                    case OPType.Update:
                        break;
                    case OPType.Delete:
                        break;
                    default:
                        break;
                }
            }
            return result;

        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await this.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public enum OPType
    {
        Insert = 1,
        Update = 2,
        Delete = 3
    }
}
