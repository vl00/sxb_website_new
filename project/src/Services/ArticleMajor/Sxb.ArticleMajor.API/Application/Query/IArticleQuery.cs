using Sxb.ArticleMajor.API.Application.Query.RequestModel;
using Sxb.ArticleMajor.API.Application.Query.ViewModel;
using Sxb.ArticleMajor.Common;
using Sxb.ArticleMajor.Common.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.API.Application.Query
{
    public interface IArticleQuery
    {
        Task<ArticleDetailVM> GetArticleDetailAsync(string code, int page = 1);
        Task<IEnumerable<ArticleItemVM>> GetLastestArticles(ArticlePlatform platform, string cityShortName, string categoryShortName, int top = 10, bool mustCover = false);
        Task<IEnumerable<ArticleItemVM>> GetLastestArticles(ArticlePlatform platform, string cityShortName, IEnumerable<int> categoryIds, int top = 10, bool mustCover = false);
        Task<PaginationArticleVM<ArticleItemVM>> GetPaginationAsync(ArticlesPaginationReqDto reqDto);
        Task<IEnumerable<ArticleItemVM>> GetRecommendArticles(ArticlePlatform platform, int cityId, string categoryName, int top = 10);
    }
}