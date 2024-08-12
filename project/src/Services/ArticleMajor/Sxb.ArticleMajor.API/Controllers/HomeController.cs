using Microsoft.AspNetCore.Mvc;
using Sxb.ArticleMajor.API.Application.Query;
using Sxb.ArticleMajor.API.Application.Query.ViewModel;
using Sxb.ArticleMajor.API.Utils;
using Sxb.ArticleMajor.Common;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.QueryDto;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RedisKeys = Sxb.ArticleMajor.Common.RedisKeys;

namespace Sxb.ArticleMajor.API.Controllers
{
    [Route("[Controller]")]
    public class HomeController : Controller
    {
        private readonly IArticleQuery _articleQuery;
        private readonly ICategoryQuery _categoryQuery;
        private readonly AppSettingsData _appSettingsData;
        private readonly IEasyRedisClient _easyRedisClient;

        public HomeController(ICategoryQuery categoryQuery, AppSettingsData appSettingsData, IEasyRedisClient easyRedisClient, IArticleQuery articleQuery)
        {
            _categoryQuery = categoryQuery;
            _appSettingsData = appSettingsData;
            _easyRedisClient = easyRedisClient;
            _articleQuery = articleQuery;
        }

        [Route("IdImages")]
        [ProducesResponseType(typeof(IdImage), 200)]
        public ResponseResult GetIdImages()
        {
            return ResponseResult.Success(Constant.IdImages);
        }
        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <returns></returns>
        [Route("cities")]
        [ProducesResponseType(typeof(CityDto), 200)]
        public ResponseResult GetCities(ArticlePlatform? platform)
        {
            if (platform == null)
                platform = ArticlePlatform.Master;

            if (platform == ArticlePlatform.GaoZhong)
            {
                return ResponseResult.Success(_appSettingsData.ImitationProvinces);
            }
            return ResponseResult.Success(_appSettingsData.Cities);
        }

        /// <summary>
        /// 获取主站主页导航
        /// </summary>
        /// <returns></returns>
        [Route("navs")]
        [ProducesResponseType(typeof(HomeCategoryVM), 200)]
        public async Task<ResponseResult> GetNavsAsync()
        {
            //var result = await _categoryQuery.GetCategoriesAsync();

            var key = RedisKeys.NavsKey;
            var result = await _easyRedisClient.GetAsync<List<HomeCategoryVM>>(key) ?? new List<HomeCategoryVM>();
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 获取子站的主页导航
        /// </summary>
        /// <param name="platform">子站id</param>
        /// <returns></returns>
        [Route("navs/sub")]
        [ProducesResponseType(typeof(Nav), 200)]
        public async Task<ResponseResult> GetSubNavsAsync(ArticlePlatform platform)
        {
            var key = string.Format(RedisKeys.SubNavsKey, platform);
            var navs = await _easyRedisClient.GetAsync<List<Nav>>(key) ?? new List<Nav>();
            if (navs.Count == 0)
            {
                navs.Add(new Nav()
                {
                    Name = "首页",
                    Url = "/"
                });
                await _easyRedisClient.AddAsync(key, navs);
            }
            return ResponseResult.Success(navs);
        }


        /// <summary>
        /// 获取子站的主页专栏配置
        /// </summary>
        /// <param name="platform">子站id</param>
        /// <param name="shortCityName">子站id</param>
        /// <returns></returns>
        [Route("navs/sub/subjects")]
        [ProducesResponseType(typeof(SubNavSubjectVM), 200)]
        public async Task<ResponseResult> GetSubNavSubjectsAsync(ArticlePlatform platform, string shortCityName)
        {
            var key = string.Format(RedisKeys.SubNavSubjectsConfigKey, platform);
            var subjects = await _easyRedisClient.GetAsync<List<SubNavSubjectVM>>(key) ?? new List<SubNavSubjectVM>();
            if (subjects.Count == 0)
            {
                subjects.Add(new SubNavSubjectVM()
                {
                    Name = "新闻动态",
                    ShortName = "xwdt",
                });
                await _easyRedisClient.AddAsync(key, subjects);
            }
            var categories = await _categoryQuery.GetCategoriesAsync(platform, shortCategoryName: "", shortCityName);
            var depth3Categories = categories.SelectMany(s => s.Children.SelectMany(c=>c.Children));

            foreach (var subject in subjects)
            {
                if (subject.Items != null)
                    subject.Items = subject.Items.Where(x => depth3Categories.Any(c => c.ShortName == x.ShortName)).ToList();
            }
            return ResponseResult.Success(subjects.Where(s=>s.Items?.Any() == true));
        }

        /// <summary>
        /// 获取主站专栏
        /// </summary>
        /// <returns></returns>
        [Route("navs/subjects")]
        [ProducesResponseType(typeof(NavSubjectVM), 200)]
        public async Task<ResponseResult> GetNavsSubjectsAsync(string shortCityName)
        {
            var data = await _easyRedisClient.GetOrUpdateAsync(RedisKeys.NavsSubjects, async () =>
            {
                var key = RedisKeys.NavsSubjectsConfig;
                var subjects = await _easyRedisClient.GetAsync<List<NavSubjectVM>>(key) ?? new List<NavSubjectVM>();
                var cityUrlPart = string.IsNullOrWhiteSpace(shortCityName) ? "gz" : shortCityName;

                foreach (var subject in subjects)
                {
                    if (subject.Items == null || subject.Items.Count == 0)
                    {
                        subject.Items = NavSubjectVM.DefaultItems();
                    }

                    bool hasSubjectShortNames = subject.ShortNames?.Any() == true;
                    foreach (var item in subject.Items)
                    {
                        //无自定义, 使用默认Url
                        if (string.IsNullOrWhiteSpace(item.Url))
                        {
                            //item.Url = $"{Constant.ArticleSites[item.Platform]}/{shortCityName}{subject.Url}";
                            item.Url = UriHelper.Combine(_appSettingsData.ArticleSites[item.Platform], cityUrlPart, subject.Url);
                        }

                        //无自定义, 使用默认Articles
                        if (item.Articles == null || item.Articles.Count == 0)
                        {
                            IEnumerable<int> categoryIds = null;
                            if (item.CategoryIds?.Any() == true)
                                categoryIds = item.CategoryIds;
                            else if (hasSubjectShortNames)
                                categoryIds = await _categoryQuery.GetCategoryIdsAsync(item.Platform, subject.ShortNames);

                            if (categoryIds?.Any() == true)
                                item.Articles = (await _articleQuery.GetLastestArticles(item.Platform, shortCityName, categoryIds, 12, mustCover: true)).ToList();
                            //使用默认数据
                            else
                                item.Articles = (await _articleQuery.GetLastestArticles(item.Platform, shortCityName, categoryShortName: null, 12, mustCover: true)).ToList();
                        }
                    }
                    //只显示有数据的
                    subject.Items = subject.Items.Where(s => s.Show).ToList();
                }
                return subjects;
            }, TimeSpan.FromMinutes(60*5));

            return ResponseResult.Success(data);
        }
    }
}
