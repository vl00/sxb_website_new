using Sxb.PointsMall.Domain.AggregatesModel.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate
{
    public interface IAccountPointsRepository: IRepository<AccountPoints>
    {
        Task AddAsync(AccountPoints accountPoints);

        Task<bool> UpdateAsync(AccountPoints accountPoints, params string[] fields);

        Task<AccountPoints> GetAsync(Guid id);


        Task<AccountPoints> FindFromAsync(Guid userId);

    }
}
