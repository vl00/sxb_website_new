using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Sxb.Recommend.Application.MapFeatureComputeRules.SchoolMapFeature
{
    public class AuthenticationComputeRule : ISchoolComputeRule
    {
        public async Task<double> Compute(School schoolP, School schoolS, MapFeature schoolMapFeature)
        {
            if (schoolP.Authentication.Any(p => schoolS.Authentication.Any(s => s.Equals(p, StringComparison.OrdinalIgnoreCase))))
            {
                return schoolMapFeature.Score * schoolMapFeature.Weight;
            }
            return await Task.FromResult(0); ;
        }
    }
}
