using Sxb.Framework.Foundation;
using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class SchoolQuotaQuery : ISchoolQuotaQuery
    {
        ISchoolQuotaRepository _schoolQuotaRepository;
        ISchoolRepository _schoolRepository;
        public SchoolQuotaQuery(ISchoolQuotaRepository schoolQuotaRepository, ISchoolRepository schoolRepository)
        {
            _schoolQuotaRepository = schoolQuotaRepository;
            _schoolRepository = schoolRepository;
        }

        public async Task<OnlineSchoolQuotaInfo> GetAsync(Guid eid, int year = 0, int type = 0)
        {
            return await _schoolQuotaRepository.GetAsync(eid, year, type);
        }

        public async Task<IEnumerable<OnlineSchoolQuotaInfo>> GetByEID(Guid eid, int year = 0, int type = 0)
        {
            if (eid == default) return null;
            var finds = await _schoolQuotaRepository.GetByEID(eid, year, type);
            if (finds?.Any() == true)
            {
                var eids = new List<Guid>();
                foreach (var item in finds)
                {
                    var guids = item.SchoolData.GetAllGuid();
                    if (guids?.Any() == true) eids.AddRange(guids);

                }
                if (eids.Any())
                {
                    eids = eids.Distinct().ToList();
                    var nos = await _schoolRepository.GetSchoolNosAsync(eids);
                    if (nos?.Any() == true)
                    {
                        foreach (var item in nos)
                        {
                            var schoolNo = UrlShortIdUtil.Long2Base32(item.Value);
                            foreach (var find in finds)
                            {
                                if (!string.IsNullOrWhiteSpace(find.SchoolData))
                                {
                                    find.SchoolData = find.SchoolData.Replace(item.Key.ToString(), schoolNo);
                                }
                            }
                        }
                    }
                }
                return finds.OrderBy(p => p.Type);
            }
            return null;
        }

        public async Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> GetYears(Guid eid)
        {
            if (eid == default) return null;
            return await _schoolQuotaRepository.GetYears(eid);
        }

        public async Task<bool> InsertAsync(OnlineSchoolQuotaInfo entity)
        {
            return await _schoolQuotaRepository.InsertAsync(entity);
        }

        public async Task<bool> RemoveByEIDAsync(Guid eid)
        {
            return await _schoolQuotaRepository.RemoveByEIDAsync(eid);
        }

        public async Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids)
        {
            return await _schoolQuotaRepository.RemoveByEIDsAsync(eids);
        }

        public async Task<bool> SaveAsync(OnlineSchoolQuotaInfo entity)
        {
            if (entity == default) return default;
            if (entity.ID == default) return await _schoolQuotaRepository.InsertAsync(entity);
            else return await _schoolQuotaRepository.UpdateAsync(entity) > 0;
        }
    }
}
