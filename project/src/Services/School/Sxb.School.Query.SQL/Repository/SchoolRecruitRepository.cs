using Dapper;
using Kogel.Dapper.Extension;
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
    public class SchoolRecruitRepository : ISchoolRecruitRepository
    {
        readonly SchoolDataDB _schoolDataDB;

        public SchoolRecruitRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<IEnumerable<OnlineSchoolRecruitInfo>> GetByEID(Guid eid, int year = 0, int type = 0)
        {
            if (eid == default) return null;
            var query = _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolRecruitInfo>().Where(p => p.EID == eid);
            if (year < 1)
            {
                var maxYear = GetRecentYear(eid);
                if (maxYear > 0) year = maxYear;
            }
            if (year > 0) query = query.Where(p => p.Year == year);
            if (type > -1) query = query.Where(p => p.Type == type);
            return await query.OrderByDescing(p => p.Year).ToIEnumerableAsync();
        }

        public async Task<IEnumerable<OnlineSchoolRecruitInfo>> GetCostByYearAsync(Guid eid, int year)
        {
            if (eid == default || year < 1900) return null;
            var str_SQL = $"Select [Year],[ID],[TuiTion],[ApplyCost],[OtherCost],[EID] From [OnlineSchoolRecruitInfo] Where [EID] = @eid AND [Year] = @year AND ([Tuition] is not null or [ApplyCost] is not null or [OtherCost] is not null)";
            var finds = await _schoolDataDB.SlaveConnection.QueryAsync<OnlineSchoolRecruitInfo>(str_SQL, new { eid, year });
            if (finds?.Any() == true) return finds;
            return null;
        }

        public async Task<IEnumerable<int>> GetCostYears(Guid eid)
        {
            if (eid == default) return null;
            var finds = await _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolRecruitInfo>().Where($"[EID] = '{eid}' AND ([Tuition] is not null or [ApplyCost] is not null or [OtherCost] is not null)").Select(p => p.Year).ToIEnumerableAsync();
            if (finds?.Any() == true)
            {
                return finds.Select(p => p.Year).Distinct().OrderByDescending(p => p);
            }
            return null;
        }

        public int GetRecentYear(Guid eid)
        {
            return _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolRecruitInfo>().Where(p => p.EID == eid).Max(p => p.Year);
            //return _schoolDataDB.QuerySet<OnlineSchoolRecruitInfo>().Where($"EID = @eid", new { eid }).Max(p => p.Year);
        }

        public async Task<IEnumerable<RecruitScheduleInfo>> GetRecruitSchedule(int cityCode, IEnumerable<int> recruitTypes, string schFType, int year, int? areaCode = null)
        {
            var query = _schoolDataDB.SlaveConnection.QuerySet<RecruitScheduleInfo>().Where(p => p.CityCode == cityCode && p.SchFType == schFType && p.Year == year).Where($"RecruitType In('{string.Join("','", recruitTypes)}')");
            if (areaCode.HasValue && areaCode.Value > 0)
            {
                query = query.Where(p => p.AreaCode == areaCode);
            }
            else
            {
                query = query.Where(p => p.AreaCode.IsNull());
            }
            return await query.ToIEnumerableAsync();
        }

        public async Task<IEnumerable<RecruitScheduleInfo>> GetRecruitScheduleByRecruitIDs(IEnumerable<Guid> recruitIDs)
        {
            if (recruitIDs == null || !recruitIDs.Any()) return null;
            return await _schoolDataDB.SlaveConnection.QuerySet<RecruitScheduleInfo>().Where($"RecruitID In('{string.Join("','", recruitIDs)}')").ToIEnumerableAsync();
        }

        public async Task<IEnumerable<int[]>> GetRecruitYears(Guid eid)
        {
            if (eid == default) return null;
            var finds = await _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolRecruitInfo>()
                .Where(p => p.EID == eid)
                .Select(p => new { p.Type, p.Year })
                .ToIEnumerableAsync();
            if (finds?.Any() == true)
            {
                return finds.Select(p => new int[] { p.Type, p.Year });
            }
            return null;
        }

        public async Task<bool> InsertAsync(OnlineSchoolRecruitInfo entity)
        {
            if (entity == null || entity.EID == default || entity.Year < 1900) return false;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return (await _schoolDataDB.Connection.CommandSet<OnlineSchoolRecruitInfo>().InsertAsync(entity)) > 0;
        }

        public async Task<int> InsertManyAsync(IEnumerable<OnlineSchoolRecruitInfo> entities)
        {
            var effectCount = 0;
            if (entities?.Any() == true)
            {
                IEnumerable<OnlineSchoolRecruitInfo> items;
                var index = 0;
                while ((items = entities.Skip(index).Take(1500))?.Any() == true)
                {
                    effectCount += await _schoolDataDB.Connection.CommandSet<OnlineSchoolRecruitInfo>().InsertAsync(items);
                    index += 1500;
                }
            }
            return effectCount;
        }

        public async Task<int> InsertRecruitScheduleAsync(RecruitScheduleInfo entity)
        {
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return await _schoolDataDB.Connection.CommandSet<RecruitScheduleInfo>().InsertAsync(entity);
        }

        public async Task<IEnumerable<OnlineSchoolRecruitInfo>> ListByEIDsAsync(IEnumerable<Guid> eids, int year = 0, int type = 0)
        {
            if (eids == default || !eids.Any()) return default;
            var query = _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolRecruitInfo>().Where($"EID in {eids.ToSQLInString()}");
            if (year > 0) query.Where(p => p.Year == year);
            if (type > 0) query.Where(p => p.Type == type);
            return await query.ToIEnumerableAsync();
        }

        public async Task<bool> RemoveByEIDAsync(Guid eid)
        {
            if (eid == default) return false;
            return (await _schoolDataDB.Connection.CommandSet<OnlineSchoolRecruitInfo>().Where(p => p.EID == eid).DeleteAsync()) > 0;
        }

        public async Task<bool> RemoveByEIDsAsync(IEnumerable<Guid> eids)
        {
            var index = 0;
            var effectCount = 0;
            IEnumerable<Guid> tmp_EIDs;
            while ((tmp_EIDs = eids.Skip(index).Take(200)).Any())
            {
                effectCount += await _schoolDataDB.Connection.CommandSet<OnlineSchoolRecruitInfo>().Where($"EID in{tmp_EIDs.ToSQLInString()}").DeleteAsync();
                index += 200;
            }

            return effectCount > 0;
        }

        public async Task<int> RemoveRecruitSchedulesAsync(IEnumerable<string> deleteParams)
        {
            var effectCount = 0;
            if (deleteParams?.Any() == true)
            {
                var index = 0;
                IEnumerable<string> tmp_Params;
                while ((tmp_Params = deleteParams.Skip(index).Take(200))?.Any() == true)
                {
                    effectCount += await _schoolDataDB.Connection.CommandSet<RecruitScheduleInfo>().Where($"ISNULL(CAST(AreaCode as VARCHAR),'') + ISNULL(SchFType,'') + ISNULL(CAST(RecruitType as VARCHAR),'') + ISNULL(CAST(CityCode as VARCHAR),'') in {tmp_Params.ToSQLInString()}").DeleteAsync();
                    index += 200;
                }
            }
            return effectCount;
        }

        public async Task<bool> UpdateAsync(OnlineSchoolRecruitInfo entity)
        {
            var result = await _schoolDataDB.Connection.CommandSet<OnlineSchoolRecruitInfo>().UpdateAsync(entity);
            return result > 0;
        }
    }
}
