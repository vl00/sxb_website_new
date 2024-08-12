using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class SchoolAchievementQuery : ISchoolAchievementQuery
    {
        ISchoolAchievementRepository _schoolAchievementRepository;
        public SchoolAchievementQuery(ISchoolAchievementRepository schoolAchievementRepository)
        {
            _schoolAchievementRepository = schoolAchievementRepository;
        }

        public async Task<IEnumerable<OnlineSchoolAchievementInfo>> GetAllAsync()
        {
            return await _schoolAchievementRepository.GetAllAsync();
        }

        public async Task<ExtensionAchievementInfo> GetAsync(Guid eid, int year = 0)
        {
            return await _schoolAchievementRepository.GetAsync(eid, year);
        }

        public async Task<IEnumerable<OnlineSchoolAchievementInfo>> GetByEID(Guid eid, int year = 0)
        {
            return await _schoolAchievementRepository.GetByEID(eid, year);
        }

        public async Task<IEnumerable<int>> GetYears(Guid eid)
        {
            return await _schoolAchievementRepository.GetYears(eid);
        }

        public async Task<bool> InsertAsync(ExtensionAchievementInfo entity)
        {
            return await _schoolAchievementRepository.InsertAsync(entity);
        }

        public async Task<IEnumerable<ExtensionAchievementInfo>> ListByEIDAsync(Guid eid, int year = 0)
        {
            return await _schoolAchievementRepository.ListByEIDAsync(eid, year);
        }

        public async Task<bool> RemoveByEIDAsync(Guid eid)
        {
            return await _schoolAchievementRepository.RemoveByEIDAsync(eid);
        }

        public async Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids)
        {
            return await _schoolAchievementRepository.RemoveByEIDsAsync(eids);
        }

        public async Task<bool> SaveAsync(ExtensionAchievementInfo entity)
        {
            if (entity == default) return false;
            if (entity.ID == default) return await _schoolAchievementRepository.InsertAsync(entity);
            else return await _schoolAchievementRepository.UpdateAsync(entity) > 0;
        }
    }
}
