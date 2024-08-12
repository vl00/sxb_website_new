using Sxb.PointsMall.Domain.AggregatesModel.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate
{
    public interface IAccountPointsItemRepository : IRepository<AccountPointsItem>
    {
        Task AddAsync(AccountPointsItem accountPoints);

        Task<bool> UpdateAsync(AccountPointsItem accountPoints, params string[] fields);

        Task<bool> DeleteAsync(AccountPointsItem accountPoints);

        Task<AccountPointsItem> GetAsync(Guid id);



    }
}
