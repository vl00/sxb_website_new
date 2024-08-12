using Sxb.Infrastructure.Core;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Infrastructure.Repositories
{
    public interface ISignInHistoryRepository : IRepository<SignInHistory, Guid>
    {
        Task<bool> ExistsHistoryAsync(string buNo, Guid userId, DateTime signInDate);
        Task<bool> ExistsUnblockHistoryAsync(string buNo, Guid userId, DateTime signInDate);
        Task<SignInHistory> GetFirstBlockHistoryAsync(string buNo, Guid userId);
        Task<IEnumerable<SignInHistory>> GetListAsync(string buNo, Guid userId);
    }
}
