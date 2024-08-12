using Sxb.School.Common.Entity;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.IRepository
{
    public interface ICorrectionMsgRepository
    {
        Task<int> InsertAsync(CorrectionMessageInfo entity);
    }
}
