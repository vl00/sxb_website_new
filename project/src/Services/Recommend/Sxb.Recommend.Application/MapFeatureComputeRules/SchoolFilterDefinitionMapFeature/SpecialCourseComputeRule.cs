using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Sxb.Recommend.Application.MapFeatureComputeRules.SchoolFilterDefinitionMapFeature
{
    public class SpecialCourseComputeRule : ISchoolFilterDefinitionComputeRule
    {

        public async Task<double> Compute(SchoolFilterDefinition filterDefinition, School school, MapFeature schoolFilterDefinitionMapFeature)
        {
            if (filterDefinition.SpecialCourse?.Any(p => school.SpecialCourse.Any(s => s.Equals(p, StringComparison.OrdinalIgnoreCase))) == true)
            {
                return await Task.FromResult(schoolFilterDefinitionMapFeature.Score * schoolFilterDefinitionMapFeature.Weight);
            }
            return await Task.FromResult(0);
        }
    }
}
