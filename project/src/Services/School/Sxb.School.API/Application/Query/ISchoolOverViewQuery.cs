using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface ISchoolOverViewQuery
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        Task<SchoolOverViewInfo> GetByEID(Guid eid);
        /// <summary>
        /// 存在则删除
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        Task<bool> DeleteIfExisted(Guid eid);

        Task<bool> InsertAsync(SchoolOverViewInfo entity);
        Task<bool> SaveAsync(SchoolOverViewInfo entity);
        Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids);
        /// <summary>
        /// 新增或插入学部的认证
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="certifications"></param>
        /// <returns></returns>
        Task<int> InsertOrUpdateCertifications(Guid eid , IEnumerable<string> certifications);
    }
}
