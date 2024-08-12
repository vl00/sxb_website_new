using Sxb.User.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.User.API.Application.Query
{
    public class SubscribeRemindQuery : ISubscribeRemindQuery
    {
        private readonly ISubscribeRemindRepository _subscribeRemindRepository;

        public SubscribeRemindQuery(ISubscribeRemindRepository subscribeRemindRepository)
        {
            _subscribeRemindRepository = subscribeRemindRepository;
        }

        public async Task<IEnumerable<Guid>> GetUserIdsAsync(string groupCode, Guid subjectId, int pageIndex, int pageSize)
        {
            return await _subscribeRemindRepository.GetUserIdsAsync(groupCode, subjectId, pageIndex, pageSize);
        }

        public async Task<bool> ExistsAndSubscribeFwhAsync(string groupCode, Guid subjectId, Guid userId)
        {
            return await _subscribeRemindRepository.ExistsAndSubscribeFwhAsync(groupCode, subjectId, userId);
        }
    }
}
