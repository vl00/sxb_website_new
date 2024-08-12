using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Sxb.Recommend.Application.MapFeatureComputeRules.SchoolMapFeature
{
    public class TypeComputeRule : ISchoolComputeRule
    {
        public async Task<double> Compute(School schoolP, School schoolS, MapFeature schoolMapFeature)
        {
        
            if (schoolP.Type == schoolS.Type)
            {
                return await Task.FromResult(schoolMapFeature.Score * schoolMapFeature.Weight) ;
            }

            return await Task.FromResult(0);
        }
    }
}
