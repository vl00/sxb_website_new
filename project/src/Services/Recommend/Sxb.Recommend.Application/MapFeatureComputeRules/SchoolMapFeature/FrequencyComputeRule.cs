using Sxb.Recommend.Application.Services;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.MapFeatureComputeRules.SchoolMapFeature
{
    public class FrequencyComputeRule : ISchoolComputeRule
    {
        ISchoolRedirectFrequencyService _schoolRedirectFrequencyService;
        public FrequencyComputeRule(ISchoolRedirectFrequencyService schoolRedirectFrequencyService)
        {
            _schoolRedirectFrequencyService = schoolRedirectFrequencyService;
        }

        public async Task<double>  Compute(School schoolP, School schoolS, MapFeature schoolMapFeature)
        {
            IEnumerable<SchoolRedirectFrequency> frequency = await _schoolRedirectFrequencyService.GetFrequenciesAsync(schoolP.Id);
            var list = frequency.OrderByDescending(f => f.OpenTime).ToList();
            int findIndex = list.FindIndex(f => f.SIdS == schoolS.Id);
            if (findIndex >= 0)
            {
                int listcount = list.Count();
                if (listcount < schoolMapFeature.FrequencyRateValues.Count)
                {
                    var afterSort = schoolMapFeature.FrequencyRateValues.OrderBy(k => k.Rate).ToList();
                    return await Task.FromResult(afterSort[findIndex].Score); 
                }
                else {
                    double[] range = schoolMapFeature.FrequencyRateValues.OrderBy(k => k.Rate).Select(k => k.Rate).ToArray();
                    double preRange = 0;
                    for (int i = 0; i < range.Length; i++)
                    {
                        int index1 = (int)(listcount * preRange);
                        int index2 = (int)(listcount * range[i]);
                        preRange = range[i];
                        if (findIndex >= index1 && findIndex < index2)
                        {
                            return await Task.FromResult(schoolMapFeature.FrequencyRateValues.FirstOrDefault(f=>f.Rate== preRange)?.Score ?? 0);
                        }
                    }
                }

            }
            return await Task.FromResult(0);

        }
    }
}
