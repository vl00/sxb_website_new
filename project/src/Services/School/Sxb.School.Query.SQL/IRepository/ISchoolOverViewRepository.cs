using Sxb.School.Common.Entity;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sxb.School.Query.SQL.IRepository
{
    public interface ISchoolOverViewRepository
    {
        Task<bool> DeleteIFExist(Guid eid);
        Task<SchoolOverViewInfo> GetByEID(Guid eid);
        Task<bool> InsertAsync(SchoolOverViewInfo entity);
        Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids);
        Task<int> UpdateAsync(SchoolOverViewInfo entity);
    }
}
