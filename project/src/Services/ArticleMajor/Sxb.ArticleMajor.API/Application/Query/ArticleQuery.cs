using Sxb.ArticleMajor.API.Application.Query.RequestModel;
using Sxb.ArticleMajor.API.Application.Query.ViewModel;
using Sxb.ArticleMajor.API.Utils;
using Sxb.ArticleMajor.Common;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.ArticleMajor.Query.Mongodb;
using Sxb.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.API.Application.Query
{
    public class ArticleQuery : IArticleQuery
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IArticleContentRepository _articleContentRepository;
        private readonly ICategoryRepository _categoryRepository;

        private readonly IEasyRedisClient _easyRedisClient;
        private readonly AppSettingsData _appSettingsData;

        public ArticleQuery(IArticleRepository articleRepository, IArticleContentRepository articleContentRepository, ICategoryRepository categoryRepository, IEasyRedisClient easyRedisClient, AppSettingsData appSettingsData)
        {
            _articleRepository = articleRepository;
            _articleContentRepository = articleContentRepository;
            _categoryRepository = categoryRepository;
            _easyRedisClient = easyRedisClient;
            _appSettingsData = appSettingsData;
        }

        /// <summary>
        /// 根据文章编码获取文章
        /// </summary>
        /// <param name="code"></param>
        /// <param name="page"></param>
        public async Task<ArticleDetailVM> GetArticleDetailAsync(string code, int page = 1)
        {
            var article = await _articleRepository.GetAsync(code);

            if (article == null || article.PageCount <= 0)
            {
                return default;
            }
            if (page <= 0 || page > article.PageCount) page = 1;

            var content = await _articleContentRepository.GetAsync(article.Id, page - 1);
            if (content == null)
            {
                throw new Exception("页面无数据");
            }

            var categories = await _categoryRepository.GetParentsAsync(article.Platform, article.CategoryId);
            var vm = new ArticleDetailVM()
            {
                Id = article.Id,
                Code = article.Code,
                Title = article.Title,
                Author = article.Author,
                FromWhere = article.FromWhere,
                PublishTime = article.PublishTime.ToString("yyyy-MM-dd"),
                Platform = article.Platform,
                Content = content.Content,
                PageCount = article.PageCount,
                Categories = categories.Select(s => (ArticleCategoryVM)s)
            };
            return vm;
        }

        public int? GetCityId(string city)
        {
            return _appSettingsData.Cities.FirstOrDefault(s => s.ShortName == city)?.Id;
        }

        /// <summary>
        /// 获取热门文章
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ArticleItemVM>> GetHotArticles(ArticlePlatform platform, string city, string category, int top = 10)
        {
            //var key = string.Format(RedisKeys.HotArticlesKey, platform, cityId, categoryId);
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取最新文章
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="cityShortName"></param>
        /// <param name="categoryShortName"></param>
        /// <param name="top"></param>
        /// <param name="mustCover"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ArticleItemVM>> GetLastestArticles(ArticlePlatform platform, string cityShortName, string categoryShortName, int top = 10, bool mustCover = false)
        {
            top = top > 100 ? 20 : top;

            var key = string.Format(RedisKeys.LastestArticlesKey, platform, cityShortName, categoryShortName, top, mustCover);
            return await _easyRedisClient.GetOrUpdateAsync(key, async () =>
            {
                var cityId = GetCityId(cityShortName);

                List<int> searchCategoryIds = new List<int>();
                if (!string.IsNullOrWhiteSpace(categoryShortName))
                {
                    searchCategoryIds = (await _categoryRepository.GetChildrenFlatWithselfAsync(platform, categoryShortName))
                        .Select(s => s.Id)
                        .ToList();

                    if (searchCategoryIds.Count == 0)
                        return new List<ArticleItemVM>();
                }

                return await GetArticleItemVM(platform, cityId, searchCategoryIds, top, mustCover);
            }, TimeSpan.FromDays(1));
        }

        private async Task<IEnumerable<ArticleItemVM>> GetArticleItemVM(ArticlePlatform platform, int? cityId, IEnumerable<int> searchCategoryIds, int top, bool mustCover)
        {
            var categories = await _categoryRepository.GetListAsync(platform);
            var articles = await _articleRepository.GetLastestArticlesAsync(platform, cityId, searchCategoryIds, top, mustCover);

            return FormatArticleItemData(platform, categories, articles);
        }

        private IEnumerable<ArticleItemVM> FormatArticleItemData(ArticlePlatform platform, IEnumerable<Category> categories, IEnumerable<Article> articles)
        {
            return articles.Select(s =>
            {
                var category = categories.FirstOrDefault(c => c.Id == s.CategoryId);
                var item = ArticleItemVM.Convert(s, category);

                //当前分类的父级->顶级
                var linkedParents = _categoryRepository.GetParentsAsync(platform, s.CategoryId, withself: true).GetAwaiter().GetResult();
                item.LinkedParents = string.Join('/', linkedParents.Skip(1).Select(s => s.ShortName));
                return item;
            });
        }

        /// <summary>
        /// 获取最新文章
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ArticleItemVM>> GetLastestArticles(ArticlePlatform platform, string cityShortName, IEnumerable<int> categoryIds, int top = 10, bool mustCover = false)
        {
            top = top > 100 ? 20 : top;

            if (!(categoryIds?.Any() == true))
            {
                throw new Exception("请选择分类");
            }

            var key = string.Format(RedisKeys.LastestArticlesKey, platform, cityShortName, string.Join('-', categoryIds), top, mustCover);
            return await _easyRedisClient.GetOrUpdateAsync(key, async () =>
            {
                var cityId = GetCityId(cityShortName);

                //所有的分类及其子分类
                var categories = await _categoryRepository.GetChildrenFlatAsync(platform, categoryIds.ToList(), withself: true);
                var searchCategoryIds = categories.Select(s => s.Id);
                if (!searchCategoryIds.Any())
                {
                    return new List<ArticleItemVM>();
                }
                return await GetArticleItemVM(platform, cityId, searchCategoryIds, top, mustCover);
            }, TimeSpan.FromDays(1));
        }


        public async Task<IEnumerable<ArticleItemVM>> GetRecommendArticles(ArticlePlatform platform, int cityId, string categoryName, int top = 10)
        {
            top = top > 100 ? 20 : top;

            var key = string.Format(RedisKeys.RecommendArticlesKey, platform, cityId, categoryName, top);
            return await _easyRedisClient.GetOrUpdateAsync(key, async () =>
            {
                //所有的分类及其子分类
                var categories = await _categoryRepository.GetChildrenFlatWithselfByNameAsync(platform, categoryName);
                var searchCategoryIds = categories.Select(s => s.Id);
               
                return await GetArticleItemVM(platform, cityId, searchCategoryIds, top, mustCover: false);
            }, TimeSpan.FromDays(1));
        }

        /// <summary>
        /// 分页获取文章列表
        /// </summary>
        public async Task<PaginationArticleVM<ArticleItemVM>> GetPaginationAsync(ArticlesPaginationReqDto reqDto)
        {
            ArticlePlatform platform = reqDto.Platform;
            string cityShortName = reqDto.CityShortName;
            string categoryShortName = reqDto.CategoryShortName;

            var cityId = GetCityId(cityShortName);
            var categories = await _categoryRepository.GetChildrenFlatWithselfAsync(platform, categoryShortName);
            if (!categories.Any())
            {
                throw new Exception("请选择分类");
            }
            var categoryIds = categories.Select(s => s.Id);

            var (articles, total) = await _articleRepository.GetArticlesAsync(platform, cityId, categoryIds, reqDto.PageIndex, reqDto.PageSize);
            var data = FormatArticleItemData(platform, categories, articles);

            //当前选择的分类
            var current = categories.FirstOrDefault(s => s.ShortName == categoryShortName);
            //当前分类的父级->顶级
            var linkedParents = await _categoryRepository.GetParentsAsync(platform, current.Id, withself: true);
            return new PaginationArticleVM<ArticleItemVM>()
            {
                Data = data,
                Total = total,
                LinkedParents = linkedParents.Skip(1).Select(s => new ArticleCategoryVM(s.Name, s.ShortName)).ToList(), //第一条是站点
                SubCategories = categories.Where(s => s.ShortName != categoryShortName).Select(s => new ArticleCategoryVM(s.Name, s.ShortName)).ToList()
            };
        }
    }
}
