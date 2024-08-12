using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.Infrastructure.Core
{
    public interface IRepository<TEntity> where TEntity : Entity, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
        TEntity Add(TEntity entity);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void AddRange(List<TEntity> entities);
        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        void UpdateRange(List<TEntity> entities);
        bool Remove(Entity entity);
        Task<bool> RemoveAsync(Entity entity);

    }



    public interface IRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : Entity<TKey>, IAggregateRoot
    {
        bool Delete(TKey id);
        Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
        TEntity Get(TKey id);
        Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> whereLambda = null);
        IQueryable<TEntity> GetAllIQueryable(Expression<Func<TEntity, bool>> whereLambda = null);
        int GetCount(Expression<Func<TEntity, bool>> whereLambd = null);
    }
}
