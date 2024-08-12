using Sxb.Infrastructure.Core;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Infrastructure.Repositories
{
    public class SignInHistoryRepository : Repository<SignInHistory, Guid, OrganizationContext>, ISignInHistoryRepository
    {
        public SignInHistoryRepository(OrganizationContext context) : base(context)
        {
        }

        public async Task<SignInHistory> GetFirstBlockHistoryAsync(string buNo, Guid userId)
        {
            var data = DbContext.Set<SignInHistory>()
                .Where(s => s.BuNo == buNo && s.MemberId == userId && s.IsValid == true && s.Blocked == true)
                .OrderBy(s => s.SignInDate)
                .FirstOrDefault();
            return await Task.FromResult(data);
        }

        public async Task<IEnumerable<SignInHistory>> GetListAsync(string buNo, Guid userId)
        {
            var data = DbContext.Set<SignInHistory>()
                .Where(s => s.BuNo == buNo && s.MemberId == userId && s.IsValid == true)
                .OrderBy(s => s.SignInDate);
            return await Task.FromResult(data);
        }

        public async Task<bool> ExistsHistoryAsync(string buNo, Guid userId, DateTime signInDate)
        {
            var data = DbContext.Set<SignInHistory>()
                .Where(s => s.BuNo == buNo && s.MemberId == userId && s.IsValid == true && s.SignInDate == signInDate.Date)
                .Any();
            return await Task.FromResult(data);
        }

        public async Task<bool> ExistsUnblockHistoryAsync(string buNo, Guid userId, DateTime signInDate)
        {
            var data = DbContext.Set<SignInHistory>()
                .Where(s => s.BuNo == buNo && s.MemberId == userId && s.IsValid == true && s.Blocked == false && s.UnblockSignInDate == signInDate.Date)
                .Any();
            return await Task.FromResult(data);
        }
    }
}
