using Sxb.Recommend.Domain;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Sxb.Recommend.Application.MapFeatureComputeRules.SchoolFilterDefinitionMapFeature
{
    public class AreaComputeRule : ISchoolFilterDefinitionComputeRule
    {

        public async Task<double> Compute(SchoolFilterDefinition filterDefinition, School school, MapFeature schoolFilterDefinitionMapFeature)
        {
            if (filterDefinition.Area != 0)
            {
                if (filterDefinition.Area == school.Area)
                {
                    return schoolFilterDefinitionMapFeature.Score * schoolFilterDefinitionMapFeature.Weight;
                }

            }

            return await Task.FromResult(0); ;
        }
    }
}
