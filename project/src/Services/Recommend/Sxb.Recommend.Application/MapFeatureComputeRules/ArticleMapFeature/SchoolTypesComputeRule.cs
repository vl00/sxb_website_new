using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.MapFeatureComputeRules.ArticleMapFeature
{
    public class SchoolTypesComputeRule : IArticleComputeRule
    {
        public async Task<double> Compute(Article articleP, Article articleS, MapFeature articleMapFeature)
        {
            if (articleP.SchoolTypes?.Any(p => articleS.SchoolTypes?.Any(s => s == p) == true) == true)
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
