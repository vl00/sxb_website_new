using Microsoft.EntityFrameworkCore;
using Sxb.Infrastructure.Core;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace Sxb.SignActivity.Infrastructure.Repositories
{
    public class SignInParentHistoryRepository : Repository<SignInParentHistory, Guid, OrganizationContext>, ISignInParentHistoryRepository
    {
        public SignInParentHistoryRepository(OrganizationContext context) : base(context)
        {
        }


        public async Task<SignInParentHistory> GetFirstBlockParentHistoryAsync(string buNo, Guid userId)
        {
            var data = DbContext.Set<SignInParentHistory>()
                .Where(s => s.BuNo == buNo && s.MemberId == userId && s.IsValid && s.Blocked)
                .OrderBy(s => s.SignInDate)
                .FirstOrDefault();
            return await Task.FromResult(data);
        }
    }
}
