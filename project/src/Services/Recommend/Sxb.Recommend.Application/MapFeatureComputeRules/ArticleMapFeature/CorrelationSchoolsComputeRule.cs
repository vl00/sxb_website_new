using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.MapFeatureComputeRules.ArticleMapFeature
{
    /// <summary>
    /// 关联学校分值计算规则
    /// </summary>
    public class CorrelationSchoolsComputeRule : IArticleComputeRule
    {
        public async Task<double> Compute(Article articleP, Article articleS, MapFeature articleMapFeature)
        {
            if (articleP.Schools?.Any(p => articleS.Schools?.Any(s => s == p) == true) == true)
            {
                return await Task.FromResult(articleMapFeature.Score * articleMapFeature.Weight);
            }
            else {
                return await Task.FromResult(0);
            }
        }
    }
}
