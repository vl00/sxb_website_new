using Sxb.User.Query.SQL.IRepository;
using System;
using System.Threading.Tasks;

namespace Sxb.User.API.Application.Query
{
    public class CollectionQuery : ICollectionQuery
    {
        readonly ICollectRepository _collectRepository;
        public CollectionQuery(ICollectRepository collectRepository)
        {
            _collectRepository = collectRepository;
        }

        public async Task<bool> CheckIsCollected(Guid dataID, Guid userID)
        {
            return await _collectRepository.CheckIsCollected(dataID, userID);
        }
    }
}
