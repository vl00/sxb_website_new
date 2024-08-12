using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class SchoolFractionQuery : ISchoolFractionQuery

    {
        ISchoolFractionRepository _schoolFractionRepository;
        public SchoolFractionQuery(ISchoolFractionRepository schoolFractionRepository)
        {
            _schoolFractionRepository = schoolFractionRepository;
        }

        public async Task<IEnumerable<SchoolFractionInfo2>> Get2AllAsync()
        {
            return await _schoolFractionRepository.Get2AllAsync();
        }

        public async Task<IEnumerable<SchoolFractionInfo2>> Get2ByEID(Guid eid, int year = 0, int type = 0)
        {
            if (eid == default) return null;
            return await _schoolFractionRepository.Get2ByEID(eid, year, type);
        }

        public async Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> Get2Years(Guid eid)
        {
            if (eid != default)
            {
                var finds = await _schoolFractionRepository.Get2Years(eid);
                if (finds?.Any() == true)
                {
                    return finds.Select(p => p.Item1).Distinct()
                    .OrderBy(p => p).Select(p => new KeyValuePair<int, IEnumerable<int>>(p, finds.Where(x => x.Item1 == p).Select(x => x.Item2).OrderByDescending(x => x)));
                }
            }
            return null;
        }

        public async Task<IEnumerable<OnlineSchoolFractionInfo>> GetAllAsync()
        {
            return await _schoolFractionRepository.GetAllAsync();
        }

        public async Task<ExtensionFractionInfo> GetAysnc(Guid eid, int year = 0, int type = 0)
        {
            return await _schoolFractionRepository.GetAsync(eid, year, type);
        }

        public async Task<IEnumerable<OnlineSchoolFractionInfo>> GetByEID(Guid eid, int year = 0)
        {
            if (eid == default) return null;
            return await _schoolFractionRepository.GetByEID(eid, year);
        }

        public async Task<IEnumerable<int>> GetYears(Guid eid)
        {
            if (eid == default) return null;
            return await _schoolFractionRepository.GetYears(eid);
        }

        public async Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> GetYearsAsync(Guid eid)
        {
            if (eid != default)
            {
                var finds = await _schoolFractionRepository.GetYearsAsync(eid);
                if (finds?.Any() == true)
                {
                    return finds.Select(p => p.Item1).Distinct()
                    .OrderBy(p => p).Select(p => new KeyValuePair<int, IEnumerable<int>>(p, finds.Where(x => x.Item1 == p).Select(x => x.Item2).OrderByDescending(x => x)));
                }
            }
            return null;
        }

        public async Task<bool> InsertAsync(ExtensionFractionInfo entity)
        {
            if (entity == default) return false;
            return await _schoolFractionRepository.InsertAsync(entity);
        }

        public async Task<IEnumerable<ExtensionFractionInfo>> ListByEIDAsync(Guid eid, int year = 0, ExtensionFractionType type = ExtensionFractionType.Unknow)
        {
            if (eid == default) return null;
            var finds = await _schoolFractionRepository.ListByEIDAsync(eid, year, type);
            if (finds?.Any() == true) return finds;
            return default;
        }

        public async Task<bool> RemoveByEIDAsync(Guid eid)
        {
            return await _schoolFractionRepository.RemoveByEIDAsync(eid);
        }

        public async Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids)
        {
            return await _schoolFractionRepository.RemoveByEIDsAsync(eids);
        }

        public async Task<bool> SaveAsync(ExtensionFractionInfo entity)
        {
            if (entity == default) return false;
            if (entity.ID == default) return await _schoolFractionRepository.InsertAsync(entity);
            else return await _schoolFractionRepository.UpdateAsync(entity) > 0;
        }
    }
}
