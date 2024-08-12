using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.ArticleMajor.Common.QueryDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb
{
    public interface IArticleRepository
    {
        Task AddAsync(IEnumerable<Article> entities);
        Task AddAsync(Article entitys);
        Task<List<string>> ExistsIdsAsync(string[] ids);
        Task<(IEnumerable<Article> data, long total)> GetArticlesAsync(ArticlePlatform platform, int? cityId, IEnumerable<int> categoryIds, int pageIndex, int pageSize);
        List<ArticleDuplication> GetArticleDuplications(int top = 0);
        Task<Article> GetAsync(string code);
        Task<List<CityCategoriesHaveData>> GetHaveDataCategoryIds(ArticlePlatform platform);
        Task<IEnumerable<Article>> GetLastestArticlesAsync(ArticlePlatform platform, int? cityId, IEnumerable<int> categoryIds, int top, bool mustCover = false);
        Task DeleteAsync(string[] ids);
        Task<List<ArticleFromUrl>> GetCodesByFromUrlAsync(string[] fromUrl);
    }
}