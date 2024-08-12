using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.IRepository
{
    public interface ISchoolProjectRepository
    {
        Task<IEnumerable<OnlineSchoolProjectInfo>> GetByEID(Guid eid);

        Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids);
        Task<int> InsertAsync(OnlineSchoolProjectInfo entity);
        Task<int> UpdateAsync(OnlineSchoolProjectInfo entity);
    }
}
