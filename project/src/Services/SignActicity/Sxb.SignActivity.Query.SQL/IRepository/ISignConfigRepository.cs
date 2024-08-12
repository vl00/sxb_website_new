using Sxb.SignActivity.Common.Entity;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Query.SQL.IRepository
{
    public interface ISignConfigRepository
    {
        Task<SignConfig> GetAsync(string buNo);
    }
}