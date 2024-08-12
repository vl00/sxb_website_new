using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.MapFeatureComputeRules.SchoolMapFeature
{
    public class SchFtypeComputeRule : ISchoolComputeRule
    {
        public async Task<double> Compute(School schoolP, School schoolS, MapFeature schoolMapFeature)
        {
            if (schoolP.SchFtype == schoolS.SchFtype)
            {
                return await Task.FromResult(schoolMapFeature.Score * schoolMapFeature.Weight);
            }

            return await Task.FromResult(0);
        }
    }
}
