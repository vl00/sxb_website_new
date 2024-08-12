using Sxb.Recommend.Domain;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.MapFeatureComputeRules
{
    public interface ISchoolFilterDefinitionComputeRule
    {

        Task<double> Compute(SchoolFilterDefinition filterDefinition, School school, MapFeature schoolFilterDefinitionMapFeature);
    }
}
