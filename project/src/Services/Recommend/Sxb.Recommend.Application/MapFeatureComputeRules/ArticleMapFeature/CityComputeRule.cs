using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.MapFeatureComputeRules.ArticleMapFeature
{
    public class CityComputeRule : IArticleComputeRule
    {
        public async Task<double> Compute(Article articleP, Article articleS, MapFeature articleMapFeature)
        {
            if (articleP.DeployAreaInfos?.Any(p => articleS.DeployAreaInfos?.Any(s => s.City == p.City) == true) == true)
            {
                return await Task.FromResult(articleMapFeature.Score * articleMapFeature.Weight);
            }
            else
            {
                return await Task.FromResult(0);
            }
        }
    }
}
