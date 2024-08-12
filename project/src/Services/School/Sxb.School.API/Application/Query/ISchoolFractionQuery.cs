using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface ISchoolFractionQuery
    {
        Task<IEnumerable<OnlineSchoolFractionInfo>> GetByEID(Guid eid, int year = 0);
        Task<IEnumerable<SchoolFractionInfo2>> Get2ByEID(Guid eid, int year = 0, int type = 0);
        Task<IEnumerable<int>> GetYears(Guid eid);
        Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> GetYearsAsync(Guid eid);
        Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> Get2Years(Guid eid);
        Task<IEnumerable<OnlineSchoolFractionInfo>> GetAllAsync();
        Task<IEnumerable<SchoolFractionInfo2>> Get2AllAsync();
        Task<bool> InsertAsync(ExtensionFractionInfo entity);
        Task<bool> SaveAsync(ExtensionFractionInfo entity);
        Task<ExtensionFractionInfo> GetAysnc(Guid eid, int year = 0, int type = 0);
        Task<IEnumerable<ExtensionFractionInfo>> ListByEIDAsync(Guid eid, int year = 0, ExtensionFractionType type = ExtensionFractionType.Unknow);
        /// <summary>
        /// 根据学部ID删除
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<bool> RemoveByEIDAsync(Guid eid);
        Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids);
    }
}
