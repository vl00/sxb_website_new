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
    public class SchoolExtensionLevelRepository : ISchoolExtensionLevelRepository
    {
        readonly SchoolDataDB _schoolDataDB;
        public SchoolExtensionLevelRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<IEnumerable<OnlineSchoolExtLevelInfo>> GetByCityCodeSchFType(int cityCode, string schFType)
        {
            return await _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolExtLevelInfo>().Where(p => p.CityCode == cityCode && p.SchFType == schFType).ToIEnumerableAsync();
        }

        public async Task<int> InsertAsync(OnlineSchoolExtLevelInfo entity)
        {
            if (!entity.CityCode.HasValue || string.IsNullOrWhiteSpace(entity.SchFType) || string.IsNullOrWhiteSpace(entity.ReplaceSource) || string.IsNullOrWhiteSpace(entity.LevelName)) return 0;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return await _schoolDataDB.Connection.CommandSet<OnlineSchoolExtLevelInfo>().InsertAsync(entity);
        }

        public async Task<int> RemoveByParamsAsync(IEnumerable<KeyValuePair<int, string>> deleteParams)
        {
            var effectCount = 0;
            if (deleteParams?.Any() == true)
            {
                var index = 0;
                IEnumerable<KeyValuePair<int, string>> tmpParams;
                while ((tmpParams = deleteParams.Skip(index).Take(200))?.Any() == true)
                {
                    effectCount += await _schoolDataDB.Connection.CommandSet<OnlineSchoolExtLevelInfo>().Where($"ISNULL(CAST(CityCode as VARCHAR),'') + ISNULL(SchFType,'') In {tmpParams.Select(p => $"{p.Key}{p.Value}").ToSQLInString()}").DeleteAsync();
                    index += 200;
                }
            }
            return effectCount;
        }
    }
}
