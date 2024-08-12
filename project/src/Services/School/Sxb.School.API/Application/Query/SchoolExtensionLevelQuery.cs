using Sxb.Framework.Cache.Redis;
using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class SchoolExtensionLevelQuery : ISchoolExtensionLevelQuery
    {
        ISchoolExtensionLevelRepository _schoolExtensionLevelRepository;
        IEasyRedisClient _easyRedisClient;
        public SchoolExtensionLevelQuery(ISchoolExtensionLevelRepository schoolExtensionLevelRepository, IEasyRedisClient easyRedisClient)
        {
            _schoolExtensionLevelRepository = schoolExtensionLevelRepository;
            _easyRedisClient = easyRedisClient;
        }
        public async Task<IEnumerable<OnlineSchoolExtLevelInfo>> GetByCityCodeSchFType(int cityCode, string schFType)
        {
            if (cityCode < 100000 || string.IsNullOrWhiteSpace(schFType)) return null;
            var finds = await _easyRedisClient.GetOrAddAsync($"OnlineSchoolExtLevelInfo:{cityCode}-{schFType}", async () =>
            {
                return await _schoolExtensionLevelRepository.GetByCityCodeSchFType(cityCode, schFType);
            }, TimeSpan.FromDays(1));
            if (finds?.Any() == true) return finds;
            return null;
        }

        public async Task<bool> InsertAsync(OnlineSchoolExtLevelInfo entity)
        {
            var result = await _schoolExtensionLevelRepository.InsertAsync(entity);
            return result > 0;
        }

        public async Task<int> RemoveByParamsAsync(IEnumerable<KeyValuePair<int, string>> deleteParams)
        {
            return await _schoolExtensionLevelRepository.RemoveByParamsAsync(deleteParams);
        }
    }
}
