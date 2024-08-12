using Sxb.Infrastructure.Core;
using Sxb.SignActivity.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Infrastructure.Repositories
{
    public interface IOrderRepository : IRepository<Order, Guid>
    {
    }
}