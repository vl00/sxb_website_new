using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class AreaRecruitPlanQuery : IAreaRecruitPlanQuery
    {
        IAreaRecruitPlanRepository _areaRecruitPlanRepository;
        public AreaRecruitPlanQuery(IAreaRecruitPlanRepository areaRecruitPlanRepository)
        {
            _areaRecruitPlanRepository = areaRecruitPlanRepository;
        }
        public async Task<IEnumerable<AreaRecruitPlanInfo>> GetByAreaCodeAndSchFType(string areaCode, string schFType, int year = 0)
        {
            if (string.IsNullOrWhiteSpace(areaCode) || string.IsNullOrWhiteSpace(schFType)) return null;
            var finds = await _areaRecruitPlanRepository.GetByAreaCodeAndSchFType(areaCode, schFType, year);
            if (finds?.Any() == true) return finds;
            return default;
        }

        public async Task<IEnumerable<int>> GetYears(string areaCode, string schFType)
        {
            return await _areaRecruitPlanRepository.GetYears(areaCode, schFType);
        }

        public async Task<bool> InsertAsync(AreaRecruitPlanInfo entity)
        {
            return await _areaRecruitPlanRepository.InsertAsync(entity);
        }

        public async Task<int> RemoveByParamsAsync(IEnumerable<string> deleteParams)
        {
            return await _areaRecruitPlanRepository.RemoveByParamsAsync(deleteParams);
        }
    }
}
