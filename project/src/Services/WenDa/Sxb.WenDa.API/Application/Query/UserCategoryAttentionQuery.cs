using Sxb.Framework.Cache.Redis;
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
    public class UserCategoryAttentionQuery : IUserCategoryAttentionQuery
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICityCategoryRepository _cityCategoryRepository;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IUserCategoryAttentionRepository _userCategoryAttentionRepository;

        private readonly AppSettingsData _appSettingsData;

        public UserCategoryAttentionQuery(ICityCategoryRepository cityCategoryRepository,
            IEasyRedisClient easyRedisClient, ICategoryRepository categoryRepository, AppSettingsData appSettingsData, IUserCategoryAttentionRepository userCategoryAttentionRepository)
        {
            _cityCategoryRepository = cityCategoryRepository;
            _easyRedisClient = easyRedisClient;
            _categoryRepository = categoryRepository;
            _appSettingsData = appSettingsData;
            _userCategoryAttentionRepository = userCategoryAttentionRepository;
        }

        /// <summary>
        /// 获取一二级分类
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AttentionCategoryDto>> GetUserCategories(Guid userId)
        {
            var key = CacheKeys.PlatformCategories;
            var data = await _easyRedisClient.GetOrAddAsync(key, async () =>
            {
                return await _categoryRepository.GetPlatformChildren();
            }, TimeSpan.FromDays(1));


            var userCategories = await _userCategoryAttentionRepository.GetUserCategoryIdsAsync(userId);
            return data.Select(s =>
            {
                var item = new AttentionCategoryDto(s.Id, s.Name)
                {
                    //_appSettingsData.ArticleSites.TryGetValue((ArticlePlatform)s.Id, out string site);
                    Children = s.Children.Select(child =>
                    {
                        return new AttentionCategoryDto(child.Id, child.Name)
                        {
                            IsAttention = userCategories.Any(c => c == child.Id)
                        };
                    }).ToList()
                };
                item.IsAttention = item.Children.Any(s => s.IsAttention);
                return item;
            });
        }
    }
}
