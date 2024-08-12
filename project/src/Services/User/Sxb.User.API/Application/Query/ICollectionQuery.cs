using System;
using System.Threading.Tasks;

namespace Sxb.User.API.Application.Query
{
    public interface ICollectionQuery
    {
        Task<bool> CheckIsCollected(Guid dataID, Guid userID);
    }
}
