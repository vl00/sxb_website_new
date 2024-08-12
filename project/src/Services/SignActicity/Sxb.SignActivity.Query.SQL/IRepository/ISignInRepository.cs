using Sxb.SignActivity.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Query.SQL.IRepository
{
    public interface ISignInRepository
    {
        Task<IEnumerable<SignIn>> GetSignInsAsync();
        Task<IEnumerable<SignIn>> GetSignInsAsync(string buNo);
    }
}
