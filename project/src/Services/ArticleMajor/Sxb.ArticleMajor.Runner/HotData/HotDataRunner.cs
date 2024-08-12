using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Query.Mongodb;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Runner.HotData
{
    internal class HotDataRunner : BaseRunner<HotDataRunner>
    {
        HttpClient _httpClient;
        ICategoryRepository _categoryRepository;
        AppSettingsData _appSettingsData;

        public HotDataRunner(HttpClient httpClient, ICategoryRepository categoryRepository, AppSettingsData appSettingsData)
        {
            _httpClient = httpClient;
            _categoryRepository = categoryRepository;
            _appSettingsData = appSettingsData;
        }

        protected override void Running()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            var categories = await _categoryRepository.GetListAsync();

            //服务端渲染, 直接调用
            var sites = _appSettingsData.ArticleSites.Where(s => 
                        s.Key == ArticlePlatform.YouEr
                        || s.Key == ArticlePlatform.XiaoXue
                        || s.Key == ArticlePlatform.ZhongXue);

            foreach (var site in sites)
            {
                foreach (var city in _appSettingsData.Cities)
                {
                    var url = UriHelper.Combine(site.Value, city.ShortName);
                    await TryGet(url);
                    await TryGet(UriHelper.Combine(site.Value, city.ShortName, "bk"));
                    await TryGet(UriHelper.Combine(site.Value, city.ShortName, "zx"));
                }
            }

            //not null
            var gaozhong = _appSettingsData.ArticleSites.FirstOrDefault(s => s.Key == ArticlePlatform.GaoZhong);
            foreach (var city in _appSettingsData.ImitationProvinces)
            {
                var url = UriHelper.Combine(gaozhong.Value, city.ShortName);
                await TryGet(url);
                await TryGet(UriHelper.Combine(gaozhong.Value, city.ShortName, "bk"));
                await TryGet(UriHelper.Combine(gaozhong.Value, city.ShortName, "zx"));
            }
        }

        private async Task TryGet(string url)
        {
            WriteLine(url);
            try
            {
                var httpResponseMessage = await _httpClient.GetAsync(url);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
