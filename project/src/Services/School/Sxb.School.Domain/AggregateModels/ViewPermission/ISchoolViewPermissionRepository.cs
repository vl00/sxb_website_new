using Sxb.School.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Domain.AggregateModels.ViewPermission
{
    public interface ISchoolViewPermissionRepository : IRepository<SchoolViewPermission>
    {

        Task AddAsync(SchoolViewPermission schoolViewPermission);

        Task<bool> UpdateAsync(SchoolViewPermission schoolViewPermission, params string[] fields);

        Task<SchoolViewPermission> GetAsync(Guid id);


        Task<SchoolViewPermission> FindFromUserAndExtAsync(Guid userId,Guid extId);


    }
}
