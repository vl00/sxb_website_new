using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.MapFeatureComputeRules.ArticleMapFeature
{
    public class TypeComputeRule : IArticleComputeRule
    {
        public async Task<double> Compute(Article articleP, Article articleS, MapFeature articleMapFeature)
        {
            if (articleP.Type == articleS.Type)
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
