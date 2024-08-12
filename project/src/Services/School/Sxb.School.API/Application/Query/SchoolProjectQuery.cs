using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class SchoolProjectQuery : ISchoolProjectQuery
    {
        ISchoolProjectRepository _schoolProjectRepository;
        public SchoolProjectQuery(ISchoolProjectRepository schoolProjectRepository)
        {
            _schoolProjectRepository = schoolProjectRepository;
        }

        public async Task<OnlineSchoolProjectInfo> GetAsync(Guid eid)
        {
            if (eid == default) return default;
            var finds = await _schoolProjectRepository.GetByEID(eid);
            if (finds?.Any() == true) return finds.First();
            return default;
        }

        public async Task<IEnumerable<OnlineSchoolProjectInfo>> GetByEID(Guid eid)
        {
            var finds = await _schoolProjectRepository.GetByEID(eid);
            if (finds?.Any() == true) return finds;
            return default;
        }

        public async Task<bool> InsertAsync(OnlineSchoolProjectInfo entity)
        {
            if (entity == default) return false;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            var result = await _schoolProjectRepository.InsertAsync(entity);
            return result > 0;
        }

        public async Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true)
            {
                return await _schoolProjectRepository.RemoveByEIDsAsync(eids.Distinct());
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> SaveAsync(OnlineSchoolProjectInfo entity)
        {
            if (entity == default) return false;
            if (entity.ID == default) return await _schoolProjectRepository.InsertAsync(entity) > 0;
            else return await _schoolProjectRepository.UpdateAsync(entity) > 0;
        }
    }
}
