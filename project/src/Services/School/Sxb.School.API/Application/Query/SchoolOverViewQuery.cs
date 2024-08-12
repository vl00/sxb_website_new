using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class SchoolOverViewQuery : ISchoolOverViewQuery
    {
        readonly ISchoolOverViewRepository _schoolOverViewRepository;
        public SchoolOverViewQuery(ISchoolOverViewRepository schoolOverViewRepository)
        {
            _schoolOverViewRepository = schoolOverViewRepository;
        }

        public async Task<bool> DeleteIfExisted(Guid eid)
        {
            if (eid == default) return false;
            return await _schoolOverViewRepository.DeleteIFExist(eid);
        }

        public async Task<SchoolOverViewInfo> GetByEID(Guid eid)
        {
            if (eid == default) return null;
            return await _schoolOverViewRepository.GetByEID(eid);
        }

        public async Task<bool> InsertAsync(SchoolOverViewInfo entity)
        {
            return await _schoolOverViewRepository.InsertAsync(entity);
        }

        public Task<int> InsertOrUpdateCertifications(Guid eid, IEnumerable<string> certifications)
        {
            throw new NotImplementedException();
        }

        public async Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids)
        {
            return await _schoolOverViewRepository.RemoveByEIDsAsync(eids);
        }

        public async Task<bool> SaveAsync(SchoolOverViewInfo entity)
        {
            if (entity == default) return false;
            if(entity.ID == default)return await _schoolOverViewRepository.InsertAsync(entity);
            else return await _schoolOverViewRepository.UpdateAsync(entity) > 0;
        }
    }
}
