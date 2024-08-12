using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class SchoolRecruitQuery : ISchoolRecruitQuery
    {
        ISchoolRecruitRepository _schoolRecruitRepository;
        public SchoolRecruitQuery(ISchoolRecruitRepository schoolRecruitRepository)
        {
            _schoolRecruitRepository = schoolRecruitRepository;
        }
        public async Task<IEnumerable<OnlineSchoolRecruitInfo>> GetByEID(Guid eid, int year = 0, int type = 0)
        {
            if (eid == default) return null;
            return await _schoolRecruitRepository.GetByEID(eid, year, type);
        }

        public async Task<IEnumerable<OnlineSchoolRecruitInfo>> ListByEIDsAsync(IEnumerable<Guid> eids, int year = 0, int type = 0)
        {
            return await _schoolRecruitRepository.ListByEIDsAsync(eids, year, type);
        }

        public async Task<IEnumerable<OnlineSchoolRecruitInfo>> GetCostByYearAsync(Guid eid, int year)
        {
            return await _schoolRecruitRepository.GetCostByYearAsync(eid, year);
        }

        public async Task<IEnumerable<int>> GetCostYears(Guid eid)
        {
            return await _schoolRecruitRepository.GetCostYears(eid);
        }

        public async Task<IEnumerable<RecruitScheduleInfo>> GetRecruitSchedules(IEnumerable<Guid> recruitIDs)
        {
            if (recruitIDs == null || !recruitIDs.Any()) return null;
            return await _schoolRecruitRepository.GetRecruitScheduleByRecruitIDs(recruitIDs);
        }

        public async Task<IEnumerable<RecruitScheduleInfo>> GetRecruitSchedules(int cityCode, IEnumerable<int> recruitTypes, string schFType, int year, int? areaCode = null)
        {
            return await _schoolRecruitRepository.GetRecruitSchedule(cityCode, recruitTypes, schFType, year, areaCode);
        }

        public async Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> GetYears(Guid eid)
        {
            var finds = await _schoolRecruitRepository.GetRecruitYears(eid);
            if (finds?.Any() == true)
            {
                return finds.Select(p => p.First()).Distinct().OrderBy(p => p).
                    Select(p => new KeyValuePair<int, IEnumerable<int>>(p, finds.Where(x => x.First() == p).Select(x => x.Last()).Distinct().OrderByDescending(x => x)));
            }
            return null;
        }

        public async Task<bool> InsertAsync(OnlineSchoolRecruitInfo entity)
        {
            return await _schoolRecruitRepository.InsertAsync(entity);
        }

        public async Task<int> InsertManyAsync(IEnumerable<OnlineSchoolRecruitInfo> entities)
        {
            return await _schoolRecruitRepository.InsertManyAsync(entities);
        }

        public async Task<int> InsertRecruitScheduleAsync(RecruitScheduleInfo entity)
        {
            return await _schoolRecruitRepository.InsertRecruitScheduleAsync(entity);
        }

        public async Task<bool> RemoveByEIDAsync(Guid eid)
        {
            return await _schoolRecruitRepository.RemoveByEIDAsync(eid);
        }

        public async Task<bool> RemoveByEIDsAsync(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true)
            {
                eids = eids.Distinct();
                return await _schoolRecruitRepository.RemoveByEIDsAsync(eids);
            }
            return false;
        }

        public async Task<int> RemoveRecruitSchedulesAsync(IEnumerable<string> deleteParams)
        {
            return await _schoolRecruitRepository.RemoveRecruitSchedulesAsync(deleteParams);
        }

        public async Task<bool> SaveAsync(OnlineSchoolRecruitInfo entity)
        {
            if (entity == default) return false;
            if (entity.ID == default) return await _schoolRecruitRepository.InsertAsync(entity);
            else return await _schoolRecruitRepository.UpdateAsync(entity);
        }
    }
}
