using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Data;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enum;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.Repositories;

namespace Sxb.WenDa.API.Application.Query
{
    public class CityCategoryQuery : ICityCategoryQuery
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICityCategoryRepository _cityCategoryRepository;
        private readonly IEasyRedisClient _easyRedisClient;

        private readonly AppSettingsData _appSettingsData;

        public CityCategoryQuery(ICityCategoryRepository cityCategoryRepository,
            IEasyRedisClient easyRedisClient, ICategoryRepository categoryRepository, AppSettingsData appSettingsData)
        {
            _cityCategoryRepository = cityCategoryRepository;
            _easyRedisClient = easyRedisClient;
            _categoryRepository = categoryRepository;
            _appSettingsData = appSettingsData;
        }

        public async Task<IEnumerable<CityDto>> GetCitys()
        {
            var result = await _easyRedisClient.GetAsync<CityDto[]>(CacheKeys.Wenda_Citys);
            if (result == null)
            {
                result = (await _cityCategoryRepository.GetCitys()).ToArray();

                await _easyRedisClient.AddAsync(CacheKeys.Wenda_Citys, result, TimeSpan.FromSeconds(60 * 60));
            }
            return result;
        }

        public async Task<GetCityCategoryQueryResult> GetCityCategory(GetCityCategoryQuery query)
        {
            var categoryId = query.CategoryId ?? 0;
            var result = new GetCityCategoryQueryResult { City = query.City, CategoryId = categoryId };

            // find
            var k = string.Format(CacheKeys.Wenda_CityCategory, query.City, categoryId);
            var datas = await _easyRedisClient.GetAsync<Category[]>(k);
            if (datas == null)
            {
                datas = (await _cityCategoryRepository.GetCategories(query.City, categoryId)).ToArray();

                await _easyRedisClient.AddAsync(k, datas, TimeSpan.FromSeconds(60 * 60 * 6));
            }
            datas ??= Array.Empty<Category>();

            // 分类
            result.ChildrenCategories = datas.Where(_ => _.Type == (byte)CategoryOrTagEnum.Category).OrderBy(_ => _.Sort)
                .Select(x => new CityCategoryItemVm
                {
                    Id = x.Id,
                    Name = x.Name,
                    CanFindSchool = x.CanFindSchool,
                })
                .ToList();
            result.ChildrenCategories = result.ChildrenCategories.Count > 0 ? result.ChildrenCategories : null;

            // 标签
            var tags = datas.Where(_ => _.Type == (byte)CategoryOrTagEnum.Tag).OrderBy(_ => _.Sort);
            if (result.ChildrenCategories != null)
            {
                result.Tags = tags.Select(x => new CityCategoryItemVm
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                    .ToList();
                result.Tags = result.Tags.Count > 0 ? result.Tags : null;
            }
            else
            {
                // 3,4级别是标签
                for (var tag = tags.FirstOrDefault(); tag != null; tag = null)
                {
                    // 2级分类的path
                    var path2 = tag.Path == null ? null : tag.Path[..^($"/{tag.Id}/".Length - 1)];
                    if (path2 == null) break;

                    k = string.Format(CacheKeys.Wenda_CityCategoryTags, path2);
                    datas = await _easyRedisClient.GetAsync<Category[]>(k);
                    if (datas == null)
                    {
                        datas = (await _cityCategoryRepository.GetCategories(query.City, path2)).ToArray();

                        await _easyRedisClient.AddAsync(k, datas, TimeSpan.FromSeconds(60 * 60 * 6));
                    }
                    datas ??= Array.Empty<Category>();

                    result.Tags = datas.Select(x => new CityCategoryItemVm
                    {
                        Id = x.Id,
                        Name = x.Name,
                    })
                        .ToList();
                    result.Tags = result.Tags.Count > 0 ? result.Tags : null;
                }
            }

            return result;
        }


        /// <summary>
        /// 获取子站及其一级子分类
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CategoryChildDto>> GetPlatformWithChildren(ArticlePlatform platform, int city)
        {
            var platformValue = platform.GetDefaultValue<long>();
            var key = string.Format(CacheKeys.PlatformCityCategories, platform, city);
            return await _easyRedisClient.GetOrAddAsync(key, async () =>
            {

                var data = await _cityCategoryRepository.GetDepth1Or2Categories(city);
                //查询指定子级
                if (platform != ArticlePlatform.Master)
                {
                    var site = _appSettingsData.GetSite(platform);
                    return data.Where(s => s.Id == platformValue)
                            .SelectMany(s => s.Children).Select(child =>
                            {
                                child.Url = $"{site}/gz/ask/list";
                                return child;
                            });
                }

                return data.Select(s =>
                {
                    var site = _appSettingsData.GetSite(s.Id);

                    s.Children.ForEach(child =>
                    {
                        child.Url = $"{site}/gz/ask/list";
                    });
                    return s;
                })
                .Where(s => s.Children.Any());
            }, TimeSpan.FromSeconds(60));
        }


        /// <summary>
        /// 获取子站的一二级分类
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CategoryChildDto>> GetChildren(ArticlePlatform platform, int city)
        {
            if (platform == ArticlePlatform.Master)
            {
                throw new ResponseResultException(nameof(platform), (int)ResponseCode.Failed);
            }

            var platformValue = platform.GetDefaultValue<long>();
            var data = await _cityCategoryRepository.GetChildrenWithChildren(city, platformValue);
            return data;
        }

        /// <summary>
        /// 获取学校分类
        /// </summary>
        /// <returns></returns>
        public async Task<Category> GetSchoolCategory(ArticlePlatform platform)
        {
            return await _cityCategoryRepository.GetSchoolCategory((long)platform);
        }
    }
}
