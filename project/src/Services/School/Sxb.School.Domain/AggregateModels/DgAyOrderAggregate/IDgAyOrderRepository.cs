using Sxb.School.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Domain.AggregateModels.DgAyOrderAggregate
{
    public interface IDgAyOrderRepository : IRepository<DgAyOrder>
    {
        Task AddAsync(DgAyOrder order);

        Task<bool> UpdateAsync(DgAyOrder order, params string[] fields);

        Task<DgAyOrder> GetAsync(Guid id);

        Task<IEnumerable<DgAyOrderDetail>> GetDgAyOrderDetailsAsync(Guid id);

    }
}
