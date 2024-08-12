using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.Foundation;
using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.DB;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.Repository
{
    public class AreaRecruitPlanRepository : IAreaRecruitPlanRepository
    {
        readonly SchoolDataDB _schoolDataDB;

        public AreaRecruitPlanRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<IEnumerable<AreaRecruitPlanInfo>> GetByAreaCodeAndSchFType(string areaCode, string schFType, int year = 0)
        {
            if (string.IsNullOrWhiteSpace(areaCode) || string.IsNullOrWhiteSpace(schFType)) return null;
            var query = _schoolDataDB.SlaveConnection.QuerySet<AreaRecruitPlanInfo>().Where(p => p.AreaCode == areaCode && p.SchFType == schFType);
            if (year < 1)
            {
                var maxYear = GetRecentYear(areaCode, schFType);
                if (maxYear > 0) year = maxYear;
            }
            if (year > 0) query = query.Where(p => p.Year == year);
            return await query.OrderByDescing(p => p.Year).ToIEnumerableAsync();
        }

        public int GetRecentYear(string areaCode, string schFType)
        {
            return _schoolDataDB.SlaveConnection.QuerySet<AreaRecruitPlanInfo>().Where(p => p.AreaCode == areaCode && p.SchFType == schFType).Max(p => p.Year);
        }

        public async Task<IEnumerable<int>> GetYears(string areaCode, string schFType)
        {
            if (string.IsNullOrWhiteSpace(areaCode) || string.IsNullOrWhiteSpace(schFType)) return null;
            return await _schoolDataDB.SlaveConnection.QuerySet<AreaRecruitPlanInfo>().Where(p => p.AreaCode == areaCode && p.SchFType == schFType).OrderByDescing(p => p.Year).Select(p => p.Year).ToIEnumerableAsync(p => p.Year);
        }

        public async Task<bool> InsertAsync(AreaRecruitPlanInfo entity)
        {
            if (entity.Year < 1900 || string.IsNullOrWhiteSpace(entity.SchFType) || string.IsNullOrWhiteSpace(entity.AreaCode)) return false;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return (await _schoolDataDB.Connection.CommandSet<AreaRecruitPlanInfo>().InsertAsync(entity)) > 0;
        }

        public async Task<int> RemoveByParamsAsync(IEnumerable<string> deleteParams)
        {
            var effectCount = 0;
            if (deleteParams?.Any() == true)
            {
                var index = 0;
                IEnumerable<string> tmp_Params;
                while ((tmp_Params = deleteParams.Skip(index).Take(200))?.Any() == true)
                {
                    effectCount += await _schoolDataDB.Connection.CommandSet<AreaRecruitPlanInfo>().Where($"ISNULL(AreaCode,'') + ISNULL(SchFType,'') + ISNULL(CAST(Year as VARCHAR),'') in {tmp_Params.ToSQLInString()}").DeleteAsync();
                    index += 200;
                }
            }
            return effectCount;
        }
    }
}
