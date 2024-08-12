using System;
using System.Threading.Tasks;

namespace Sxb.User.Query.SQL.IRepository
{
    public interface ICollectRepository
    {
        Task<bool> CheckIsCollected(Guid dataID, Guid userID);
    }
}
