using Sxb.SignActivity.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Query.SQL.IRepository
{
    public interface ISignInHistoryRepository
    {
        Task<IEnumerable<SignInHistory>> GetListAsync(string buNo, DateTime signInDate);
        Task<IEnumerable<SignInHistory>> GetListAsync(string buNo);
    }
}
