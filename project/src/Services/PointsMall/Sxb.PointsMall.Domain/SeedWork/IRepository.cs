using Sxb.Domain;
using Sxb.PointsMall.Domain.SeedWork;

namespace Sxb.PointsMall.Domain.AggregatesModel.SeedWork
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
