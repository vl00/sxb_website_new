using Microsoft.EntityFrameworkCore;
using Sxb.Infrastructure.Core;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Sxb.SignActivity.Infrastructure.Repositories
{
    public class SignInRepository : Repository<SignIn, Guid, OrganizationContext>, ISignInRepository
    {
        public SignInRepository(OrganizationContext context) : base(context)
        {
        }

        public async Task<SignIn> GetUserSignAsync(string buNo, Guid userId)
        {
            var data = FirstOrDefault(s => s.BuNo == buNo && s.MemberId == userId && s.IsValid);
            return await Task.FromResult(data);
        }

        public int GetSignUserCount(string buNo)
        {
            return GetCount(s => s.BuNo == buNo && s.IsValid);
        }
    }
}
