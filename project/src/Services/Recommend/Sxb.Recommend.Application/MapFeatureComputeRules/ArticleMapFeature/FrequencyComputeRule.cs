using Sxb.Recommend.Application.Services;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.MapFeatureComputeRules.ArticleMapFeature
{
    public class FrequencyComputeRule : IArticleComputeRule
    {
        IArticleRedirectFrequencyService _articleRedirectFrequencyService;
        public FrequencyComputeRule(IArticleRedirectFrequencyService articleRedirectFrequencyService)
        {
            _articleRedirectFrequencyService = articleRedirectFrequencyService;
        }


        public async Task<double> Compute(Article articleP, Article articleS, MapFeature mapfeature)
        {
            IEnumerable<ArticleRedirectFrequency> frequency = await _articleRedirectFrequencyService.GetFrequenciesAsync(articleP.Id);
            var list = frequency.OrderByDescending(f => f.OpenTimes).ToList();
            int findIndex = list.FindIndex(f => f.AIdS == articleS.Id);
            if (findIndex >= 0)
            {
                int listcount = list.Count();
                if (listcount < mapfeature.FrequencyRateValues.Count)
                {
                    var afterSort = mapfeature.FrequencyRateValues.OrderBy(k => k.Rate).ToList();
                    return await Task.FromResult(afterSort[findIndex].Score);
                }
                else
                {
                    double[] range = mapfeature.FrequencyRateValues.OrderBy(k => k.Rate).Select(k => k.Rate).ToArray();
                    double preRange = 0;
                    for (int i = 0; i < range.Length; i++)
                    {
                        int index1 = (int)(listcount * preRange);
                        int index2 = (int)(listcount * range[i]);
                        preRange = range[i];
                        if (findIndex >= index1 && findIndex < index2)
                        {
                            return await Task.FromResult(mapfeature.FrequencyRateValues.FirstOrDefault(f => f.Rate == preRange)?.Score ?? 0);
                        }
                    }
                }


            }
            return await Task.FromResult(0);
        }
    }
}
