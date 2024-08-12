using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.Foundation;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Query.SQL.DB;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.Repository
{
    public class SchoolFractionRepository : ISchoolFractionRepository
    {
        readonly SchoolDataDB _schoolDataDB;

        public SchoolFractionRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<IEnumerable<SchoolFractionInfo2>> Get2ByEID(Guid eid, int year = 0, int type = 0)
        {
            if (eid == default) return null;
            var query = _schoolDataDB.SlaveConnection.QuerySet<SchoolFractionInfo2>().Where(p => p.EID == eid);
            if (year < 1)
            {
                var recentYear = await Get2RecentYear(eid);
                if (recentYear > 0) year = recentYear;
            }
            if (year > 0) query = query.Where(p => p.Year == year);
            if (type > 0) query = query.Where(p => p.Type == type);
            return await query.OrderByDescing(p => p.Year).ToIEnumerableAsync();
        }

        public async Task<IEnumerable<(int, int)>> Get2Years(Guid eid)
        {
            if (eid != default)
            {
                var finds = await _schoolDataDB.SlaveConnection.QuerySet<SchoolFractionInfo2>().Where(p => p.EID == eid).Select(p => new { p.Year, p.Type }).ToIEnumerableAsync();
                if (finds?.Any() == true)
                {
                    return finds.Select(p => (p.Type, p.Year));
                }
            }
            return null;
        }

        public async Task<IEnumerable<OnlineSchoolFractionInfo>> GetByEID(Guid eid, int year = 0)
        {
            if (eid == Guid.Empty) return null;
            var query = _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolFractionInfo>().Where(p => p.EID == eid);
            if (year < 1)
            {
                var recentYear = await GetRecentYear(eid);
                if (recentYear > 0) year = recentYear;
            }
            if (year > 0) query = query.Where(p => p.Year == year);
            return await query.OrderByDescing(p => p.Year).ToIEnumerableAsync();
        }
        public async Task<int> GetRecentYear(Guid eid)
        {
            if (eid == default) return 0;
            return await Task.Run(() =>
            {
                return _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolFractionInfo>().Where(p => p.EID == eid).Max(p => p.Year);
            });
        }

        public async Task<int> Get2RecentYear(Guid eid)
        {
            if (eid == default) return 0;
            return await Task.Run(() =>
            {
                return _schoolDataDB.SlaveConnection.QuerySet<ExtensionFractionInfo>().Where(p => p.EID == eid).Max(p => p.Year);
            });
        }

        public async Task<IEnumerable<int>> GetYears(Guid eid)
        {
            if (eid != default)
            {
                var finds = await _schoolDataDB.SlaveConnection.QuerySet<ExtensionFractionInfo>().Where(p => p.EID == eid).Select(p => p.Year).ToIEnumerableAsync();
                if (finds?.Any() == true)
                {
                    return finds.Select(p => p.Year).Distinct().OrderByDescending(p => p);
                }
            }
            return null;
        }

        public async Task<IEnumerable<OnlineSchoolFractionInfo>> GetAllAsync()
        {
            return await _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolFractionInfo>().ToIEnumerableAsync();
        }

        public async Task<bool> InsertAsync(ExtensionFractionInfo entity)
        {
            if (entity == default) return false;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return (await _schoolDataDB.Connection.CommandSet<ExtensionFractionInfo>().InsertAsync(entity)) > 0;
        }

        public async Task<IEnumerable<SchoolFractionInfo2>> Get2AllAsync()
        {
            return await _schoolDataDB.SlaveConnection.QuerySet<SchoolFractionInfo2>().ToIEnumerableAsync();
        }

        public async Task<IEnumerable<ExtensionFractionInfo>> ListByEIDAsync(Guid eid, int year = 0, ExtensionFractionType type = ExtensionFractionType.Unknow)
        {
            var query = _schoolDataDB.SlaveConnection.QuerySet<ExtensionFractionInfo>().Where(p => p.EID == eid);
            if (year < 1)
            {
                var recentYear = await GetMaxYearAsync(eid);
                if (recentYear > 0) year = recentYear;
            }
            if (year > 0) query = query.Where(p => p.Year == year);
            if (type > 0) query = query.Where(p => p.Type == type);
            return await query.OrderByDescing(p => p.Year).ToIEnumerableAsync();
        }

        public async Task<bool> RemoveByEIDAsync(Guid eid)
        {
            if (eid == default) return false;
            return (await _schoolDataDB.Connection.CommandSet<ExtensionFractionInfo>().Where(p => p.EID == eid).DeleteAsync()) > 0;
        }

        public async Task<int> GetMaxYearAsync(Guid eid)
        {
            if (eid == default) return 0;
            return await Task.Run(() =>
            {
                return _schoolDataDB.SlaveConnection.QuerySet<ExtensionFractionInfo>().Where(p => p.EID == eid).Max(p => p.Year);
            });
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
                    effectCount += await _schoolDataDB.Connection.CommandSet<ExtensionFractionInfo>().Where($"EID IN {ids.ToSQLInString()}").DeleteAsync();
                    index += 200;
                }
            }
            return effectCount;
        }

        public async Task<IEnumerable<(int, int)>> GetYearsAsync(Guid eid)
        {
            if (eid != default)
            {
                var finds = await _schoolDataDB.SlaveConnection.QuerySet<ExtensionFractionInfo>().Where(p => p.EID == eid).Select(p => new { p.Year, p.Type }).ToIEnumerableAsync();
                if (finds?.Any() == true)
                {
                    return finds.Select(p => ((int)p.Type, p.Year));
                }
            }
            return null;
        }

        public async Task<int> UpdateAsync(ExtensionFractionInfo entity)
        {
            if (entity == default || entity.ID == default) return 0;
            return await _schoolDataDB.Connection.CommandSet<ExtensionFractionInfo>().UpdateAsync(entity);
        }

        public async Task<ExtensionFractionInfo> GetAsync(Guid eid, int year = 0, int type = 0)
        {
            if (eid == default) return default;
            var query = _schoolDataDB.SlaveConnection.QuerySet<ExtensionFractionInfo>().Where(p => p.EID == eid);
            if (year > 0) query.Where(p => p.Year == year);
            if (type > 0) query.Where($"Type = {type}");
            var find = await query.GetAsync();
            if (find?.ID != default) return find;
            return default;
        }
    }
}