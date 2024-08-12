using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.Foundation;
using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.DB;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.Repository
{
    public class SchoolAchievementRepository : ISchoolAchievementRepository
    {
        readonly SchoolDataDB _schoolDataDB;

        public SchoolAchievementRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<IEnumerable<OnlineSchoolAchievementInfo>> GetAllAsync()
        {
            return await _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolAchievementInfo>().ToIEnumerableAsync();
        }

        public async Task<ExtensionAchievementInfo> GetAsync(Guid eid, int year = 0)
        {
            if (eid == default) return default;
            var query = _schoolDataDB.SlaveConnection.QuerySet<ExtensionAchievementInfo>().Where(p => p.EID == eid);
            if (year > 0) query.Where(p => p.Year == year);
            var find = await query.GetAsync();
            if (find?.ID != default) return find;
            return default;
        }

        public async Task<IEnumerable<OnlineSchoolAchievementInfo>> GetByEID(Guid eid, int year = 0)
        {
            if (eid == default) return null;
            var query = _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolAchievementInfo>().Where(p => p.EID == eid);
            if (year < 1)
            {
                var recentYear = await GetRecentYear(eid);
                if (recentYear > 0) year = recentYear;
            }
            if (year > 0) query = query.Where(p => p.Year == year);
            return await query.OrderByDescing(p => p.Year).ToIEnumerableAsync();
        }

        public async Task<int> GetMaxYearAsync(Guid eid)
        {
            if (eid == default) return 0;
            return await Task.Run(() =>
            {
                return _schoolDataDB.SlaveConnection.QuerySet<ExtensionAchievementInfo>().Where(p => p.EID == eid).Max(p => p.Year);
            });
        }

        public async Task<int> GetRecentYear(Guid eid)
        {
            if (eid == default) return 0;
            return await Task.Run(() =>
            {
                return _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolAchievementInfo>().Where(p => p.EID == eid).Max(p => p.Year);
            });
        }

        public async Task<IEnumerable<int>> GetYears(Guid eid)
        {
            if (eid == default) return null;
            var finds = await _schoolDataDB.SlaveConnection.QuerySet<ExtensionAchievementInfo>().Where(p => p.EID == eid).Select(p => p.Year).ToIEnumerableAsync();
            if (finds?.Any() == true)
            {
                return finds.Select(p => p.Year).Distinct();
            }
            return null;
        }

        public async Task<bool> InsertAsync(ExtensionAchievementInfo entity)
        {
            if (entity.EID == default || entity.Year < 1900) return false;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return (await _schoolDataDB.Connection.CommandSet<ExtensionAchievementInfo>().InsertAsync(entity)) > 0;
        }

        public async Task<IEnumerable<ExtensionAchievementInfo>> ListByEIDAsync(Guid eid, int year = 0)
        {
            if (eid == default) return null;
            var query = _schoolDataDB.SlaveConnection.QuerySet<ExtensionAchievementInfo>().Where(p => p.EID == eid);
            if (year < 1)
            {
                var recentYear = await GetMaxYearAsync(eid);
                if (recentYear > 0) year = recentYear;
            }
            if (year > 0) query = query.Where(p => p.Year == year);
            return await query.OrderByDescing(p => p.Year).ToIEnumerableAsync();
        }

        public async Task<bool> RemoveByEIDAsync(Guid eid)
        {
            if (eid == default) return false;
            return (await _schoolDataDB.Connection.CommandSet<ExtensionAchievementInfo>().Where(p => p.EID == eid).DeleteAsync()) > 0;
        }

        public async Task<int> RemoveByEIDsAsync(IEnumerable<Guid> eids)
        {
            var effectCount = 0;
            if (eids?.Any() == true)
            {
                var index = 0;
                IEnumerable<Guid> ids;
                while ((ids = eids.Skip(index).Take(200))?.Any() == true)
                {
                    effectCount += await _schoolDataDB.Connection.CommandSet<ExtensionAchievementInfo>().Where($"EID IN {ids.ToSQLInString()}").DeleteAsync();
                    index += 200;
                }
            }
            return effectCount;
        }

        public async Task<int> UpdateAsync(ExtensionAchievementInfo entity)
        {
            if (entity == default || entity.ID == default) return 0;
            return await _schoolDataDB.Connection.CommandSet<ExtensionAchievementInfo>().UpdateAsync(entity);
        }
    }
}
