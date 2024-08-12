using Sxb.ArticleMajor.API.Application.Query.ViewModel;
using Sxb.ArticleMajor.API.Utils;
using Sxb.ArticleMajor.Common;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.QueryDto;
using Sxb.ArticleMajor.Query.Mongodb;
using Sxb.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.API.Application.Query
{
    public class CategoryQuery : ICategoryQuery
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly AppSettingsData _appSettingsData;

        public CategoryQuery(ICategoryRepository categoryRepository, IEasyRedisClient easyRedisClient, AppSettingsData appSettingsData, IArticleRepository articleRepository)
        {
            _categoryRepository = categoryRepository;
            _easyRedisClient = easyRedisClient;
            _appSettingsData = appSettingsData;
            _articleRepository = articleRepository;
        }

        /// <summary>
        /// 获取主站导航 - 子站点及其一级分类
        /// </summary>
        public async Task<IEnumerable<HomeCategoryVM>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetChildrenAsync();
            return categories.Select(item =>
            {
                var baseUrl = _appSettingsData.ArticleSites[item.Platform];
                //默认广州
                var url = string.Format("{0}/gz", baseUrl, item.ShortName);
                return new HomeCategoryVM()
                {
                    Name = item.Name,
                    Url = url,
                    Children = item.Children.Select(s => new HomeCategoryVM()
                    {
                        Name = s.Name,
                        Url = string.Format("{0}/{1}", url, s.ShortName)
                    })
                };
            });
        }


        public int? GetCityId(string city)
        {
            return _appSettingsData.Cities.FirstOrDefault(s => s.ShortName == city)?.Id;
        }

        /// <summary>
        /// 获取子站导航
        /// </summary>
        public async Task<IEnumerable<CategoryQueryDto>> GetCategoriesAsync(ArticlePlatform platform, string shortCategoryName, string shortCityName)
        {

            //默认0查询整个目录
            int parentId = 0;
            if (!string.IsNullOrWhiteSpace(shortCategoryName))
            {
                var category = await _categoryRepository.GetAsync(platform, shortCategoryName);
                if (category == null)
                {
                    throw new System.Exception("类型错误");
                }
                parentId = category.Id;
            }


            List<int> haveDataLeafCategories = null;
            if (!string.IsNullOrWhiteSpace(shortCityName))
            {
                haveDataLeafCategories = await GetHaveDataCategoryIdsAsync(platform, shortCityName);
            }

            var categories = await _categoryRepository.GetChildrenAsync(platform, parentId, haveDataLeafCategories);
            return categories;
        }

        public async Task<List<int>> GetHaveDataCategoryIdsAsync(ArticlePlatform platform, string shortCityName)
        {
            //不同城市不同分类
            int cityId = GetCityId(shortCityName) ?? 0;
            if (cityId == 0)
            {
                throw new System.Exception("城市错误");
            }

            var key = string.Format(RedisKeys.HaveDataCityCategories, platform);
            var data = await _easyRedisClient.GetOrUpdateAsync(key, async () =>
            {
                return await _articleRepository.GetHaveDataCategoryIds(platform);
            }, TimeSpan.FromDays(1));
            return CityCategoriesHaveData.GetCategoryIds(data, cityId);
        }

        /// <summary>
        /// 获取分类名称
        /// </summary>
        public async Task<ArticleCategoryVM> GetCategoryAsync(ArticlePlatform platform, string shortCategoryName)
        {
            if (string.IsNullOrWhiteSpace(shortCategoryName))
            {
                return null;
            }
            var category = await _categoryRepository.GetAsync(platform, shortCategoryName);
            if (category == null)
            {
                return null;
            }

            return new ArticleCategoryVM(category.Name, category.ShortName);
        }

        /// <summary>
        /// 获取分类id
        /// </summary>
        public async Task<IEnumerable<int>> GetCategoryIdsAsync(ArticlePlatform platform, IEnumerable<string> shortCategoryNames)
        {
            var categories = await _categoryRepository.GetListAsync(platform, shortCategoryNames);
            return categories.Select(s => s.Id);
        }
    }
}
