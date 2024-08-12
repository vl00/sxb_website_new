using Sxb.Recommend.Domain;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  Sxb.Recommend.Application.MapFeatureComputeRules
{
    public interface IArticleComputeRule
    {

        Task<double> Compute(Article articleP, Article articleS, MapFeature articleMapFeature);
    }
}
