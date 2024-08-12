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
    public class SchoolQuotaRepository : ISchoolQuotaRepository
    {
        readonly SchoolDataDB _schoolDataDB;

        public SchoolQuotaRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<OnlineSchoolQuotaInfo> GetAsync(Guid eid, int year = 0, int type = 0)
        {
            if (eid == default) return default;
            var query = _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolQuotaInfo>().Where(p => p.EID == eid);
            if (year > 0) query.Where(p => p.Year == year);
            if (type > 0) query.Where(p => p.Type == type);
            var find = await query.GetAsync();
            if (find?.ID != default) return find;
            return default;
        }

        public async Task<IEnumerable<OnlineSchoolQuotaInfo>> GetByEID(Guid eid, int year = 0, int type = 0)
        {
            if (eid == Guid.Empty) return null;
            var query = _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolQuotaInfo>().Where(p => p.EID == eid);
            if (year < 1)
            {
                var recentYear = await GetRecentYear(eid);
                if (recentYear > 0) year = recentYear;
            }
            if (year > 0) query = query.Where(p => p.Year == year);
            if (type > 0) query = query.Where(p => p.Type == type);
            return await query.OrderByDescing(p => p.Year).ToIEnumerableAsync();
        }

        public async Task<int> GetRecentYear(Guid eid)
        {
            if (eid == default) return 0;
            return await Task.Run(() =>
            {
                return _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolQuotaInfo>().Where(p => p.EID == eid).Max(p => p.Year);
            });
        }

        public async Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> GetYears(Guid eid)
        {
            if (eid != default)
            {
                var finds = await _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolQuotaInfo>().Where(p => p.EID == eid).Select(p => new { p.Type, p.Year }).ToIEnumerableAsync();
                if (finds?.Any() == true)
                {
                    return finds.Select(p => p.Type).Distinct().OrderBy(p => p).
                        Select(p => new KeyValuePair<int, IEnumerable<int>>(p, finds.Where(x => x.Type == p).Select(x => x.Year).Distinct().OrderByDescending(x => x)));
                }
            }
            return null;
        }

        public async Task<bool> InsertAsync(OnlineSchoolQuotaInfo entity)
        {
            if (entity == null || entity.EID == default || entity.Year < 1900) return false;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return (await _schoolDataDB.Connection.CommandSet<OnlineSchoolQuotaInfo>().InsertAsync(entity)) > 0;
        }

        public async Task<bool> RemoveByEIDAsync(Guid eid)
        {
            if (eid == default) return false;
            return (await _schoolDataDB.Connection.CommandSet<OnlineSchoolQuotaInfo>().Where(p => p.EID == eid).DeleteAsync()) > 0;
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
                    effectCount += await _schoolDataDB.Connection.CommandSet<OnlineSchoolQuotaInfo>().Where($"EID IN{ids.ToSQLInString()}").DeleteAsync();
                    index += 200;
                }
            }
            return effectCount;
        }

        public async Task<int> UpdateAsync(OnlineSchoolQuotaInfo entity)
        {
            if (entity == default || entity.ID == default) return 0;
            return await _schoolDataDB.Connection.CommandSet<OnlineSchoolQuotaInfo>().UpdateAsync(entity);
        }
    }
}
