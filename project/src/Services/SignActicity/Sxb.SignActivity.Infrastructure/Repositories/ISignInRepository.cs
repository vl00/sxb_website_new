using Sxb.Infrastructure.Core;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Infrastructure.Repositories
{
    public interface ISignInRepository : IRepository<SignIn, Guid>
    {
        int GetSignUserCount(string buNo);
        Task<SignIn> GetUserSignAsync(string buNo, Guid userId);
    }
}
