using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enum;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.Repositories;

namespace Sxb.WenDa.API.Application.Query
{
    public interface ICityCategoryQuery
    {
        /// <summary>获取(已开通的)城市</summary>
        Task<IEnumerable<CityDto>> GetCitys();
        /// <summary>查城市下的分类+标签</summary>
        Task<GetCityCategoryQueryResult> GetCityCategory(GetCityCategoryQuery query);
        Task<IEnumerable<CategoryChildDto>> GetPlatformWithChildren(Common.Enum.ArticlePlatform platform, int city);
        Task<Category> GetSchoolCategory(ArticlePlatform platform);
        Task<IEnumerable<CategoryChildDto>> GetChildren(ArticlePlatform platform, int city);
    }
}