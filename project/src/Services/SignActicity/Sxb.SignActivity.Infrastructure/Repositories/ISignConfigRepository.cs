using Sxb.Infrastructure.Core;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using System;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Infrastructure.Repositories
{
    public interface ISignConfigRepository : IRepository<SignConfig, Guid>
    {
        Task<SignConfig> GetAsync(string buNo);
    }
}