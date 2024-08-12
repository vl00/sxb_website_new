using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Sxb.Recommend.Application.MapFeatureComputeRules.SchoolMapFeature
{
    public class ScoreComputeRule : ISchoolComputeRule
    {
        public async Task<double> Compute(School schoolP, School schoolS, MapFeature schoolMapFeature)
        {
            if (GetGrade(schoolP.Score) == GetGrade(schoolS.Score))
            {
                return schoolMapFeature.Score * schoolMapFeature.Weight;
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
