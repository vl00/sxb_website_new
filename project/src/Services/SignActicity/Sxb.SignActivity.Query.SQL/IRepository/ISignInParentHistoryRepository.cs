using Sxb.SignActivity.Common.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Query.SQL.IRepository
{
    public interface ISignInParentHistoryRepository
    {
        Task<IEnumerable<SignInParentHistory>> GetListAsync(string buNo);
    }
}