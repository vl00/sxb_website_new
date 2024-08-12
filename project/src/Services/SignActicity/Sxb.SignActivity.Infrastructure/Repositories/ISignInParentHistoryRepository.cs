using Sxb.Infrastructure.Core;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using System;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Infrastructure.Repositories
{
    public interface ISignInParentHistoryRepository : IRepository<SignInParentHistory, Guid>
    {
        Task<SignInParentHistory> GetFirstBlockParentHistoryAsync(string buNo, Guid userId);
    }
}