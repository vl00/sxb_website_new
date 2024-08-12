using Sxb.Recommend.Domain;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Sxb.Recommend.Application.MapFeatureComputeRules
{
    public interface ISchoolComputeRule
    {

        Task<double> Compute(School schoolP, School schoolS,MapFeature schoolMapFeature);
    }
}
