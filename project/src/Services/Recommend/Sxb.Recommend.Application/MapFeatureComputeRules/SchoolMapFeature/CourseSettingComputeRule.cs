using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Sxb.Recommend.Application.MapFeatureComputeRules.SchoolMapFeature
{
    public class CourseSettingComputeRule : ISchoolComputeRule
    {
        public async Task<double> Compute(School schoolP, School schoolS, MapFeature schoolMapFeature)
        {

            if (schoolP.CourseSetting.Any(p => schoolS.CourseSetting.Any(s => s.Equals(p, StringComparison.OrdinalIgnoreCase))))
            {
                return schoolMapFeature.Score * schoolMapFeature.Weight;
            }
            return await Task.FromResult(0);
        }
    }
}
