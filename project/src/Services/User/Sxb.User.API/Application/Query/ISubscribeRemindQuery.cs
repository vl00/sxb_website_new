using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.User.API.Application.Query
{
    public interface ISubscribeRemindQuery
    {
        Task<bool> ExistsAndSubscribeFwhAsync(string groupCode, Guid subjectId, Guid userId);
        Task<IEnumerable<Guid>> GetUserIdsAsync(string groupCode, Guid subjectId, int pageIndex, int pageSize);
    }
}