using Sxb.SignActivity.Common.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.SignActivity.API.Application.Query
{
    public interface ISignQuery
    {
        Task<IEnumerable<SignIn>> GetSignInsAsync();
    }
}
