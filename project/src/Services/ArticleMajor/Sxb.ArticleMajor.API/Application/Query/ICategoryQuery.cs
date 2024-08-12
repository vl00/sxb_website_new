using Sxb.ArticleMajor.API.Application.Query.ViewModel;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.QueryDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.API.Application.Query
{
    public interface ICategoryQuery
    {
        Task<IEnumerable<HomeCategoryVM>> GetCategoriesAsync();
        Task<IEnumerable<CategoryQueryDto>> GetCategoriesAsync(ArticlePlatform platform, string shortCategoryName, string shortCityName);
        Task<ArticleCategoryVM> GetCategoryAsync(ArticlePlatform platform, string shortCategoryName);
        Task<IEnumerable<int>> GetCategoryIdsAsync(ArticlePlatform platform, IEnumerable<string> shortCategoryNames);
        Task<List<int>> GetHaveDataCategoryIdsAsync(ArticlePlatform platform, string shortCityName);
    }
}