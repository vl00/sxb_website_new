using Sxb.School.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Domain.AggregateModels.ViewOrder
{
    public interface ISchoolViewOrderRepository : IRepository<SchoolViewOrder>
    {

        Task AddAsync(SchoolViewOrder  schoolViewOrder);

        Task<bool> UpdateAsync(SchoolViewOrder schoolViewOrder, params string[] fields);

        Task<SchoolViewOrder> GetAsync(Guid id);

    }
}
