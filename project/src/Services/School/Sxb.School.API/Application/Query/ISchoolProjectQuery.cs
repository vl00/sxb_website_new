using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface ISchoolProjectQuery
    {
        Task<IEnumerable<OnlineSchoolProjectInfo>> GetByEID(Guid eid);

        Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids);
        Task<bool> InsertAsync(OnlineSchoolProjectInfo entity);
        Task<bool> SaveAsync(OnlineSchoolProjectInfo entity);
        Task<OnlineSchoolProjectInfo> GetAsync(Guid eid);
    }
}
