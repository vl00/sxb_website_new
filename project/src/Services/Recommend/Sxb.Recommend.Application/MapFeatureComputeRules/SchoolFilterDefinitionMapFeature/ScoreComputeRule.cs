using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Sxb.Recommend.Application.MapFeatureComputeRules.SchoolFilterDefinitionMapFeature
{
    public class ScoreComputeRule : ISchoolFilterDefinitionComputeRule
    {
        public async Task<double> Compute(SchoolFilterDefinition filterDefinition, School school, MapFeature schoolFilterDefinitionMapFeature)
        {
            if (GetGrade(filterDefinition.Score) == GetGrade(school.Score))
            {
                return schoolFilterDefinitionMapFeature.Score * schoolFilterDefinitionMapFeature.Weight;
            }

            return await Task.FromResult(0);
        }


        string GetGrade(double score)
        {
            if (score >= 90)
            {
                return "A+";
            }
            else if (score >= 80 && score < 90)
            {
                return "A";
            }
            else if (score >= 70 && score < 80)
            {
                return "B";
            }
            else if (score >= 60 && score < 70)
            {
                return "C";
            }
            else
            {
                return "D";
            }
        }
    }
}
