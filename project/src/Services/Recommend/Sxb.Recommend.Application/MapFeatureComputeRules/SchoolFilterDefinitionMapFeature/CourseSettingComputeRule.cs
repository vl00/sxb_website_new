using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Sxb.Recommend.Application.MapFeatureComputeRules.SchoolFilterDefinitionMapFeature
{
    public class CourseSettingComputeRule : ISchoolFilterDefinitionComputeRule
    {

        public async Task<double> Compute(SchoolFilterDefinition filterDefinition, School school, MapFeature schoolFilterDefinitionMapFeature)
        {
            if (filterDefinition.CourseSetting?.Any(p => school.CourseSetting.Any(s => s.Equals(p, StringComparison.OrdinalIgnoreCase))) == true)
            {
                return schoolFilterDefinitionMapFeature.Score * schoolFilterDefinitionMapFeature.Weight;
            }
            return await Task.FromResult(0);
        }
    }
}
