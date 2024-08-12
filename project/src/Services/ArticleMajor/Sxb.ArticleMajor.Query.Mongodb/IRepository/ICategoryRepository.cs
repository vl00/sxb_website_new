using MongoDB.Driver;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.ArticleMajor.Common.QueryDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb
{
    public interface ICategoryRepository
    {
        Task AddAsync(IEnumerable<Category> entities);
        Task<IEnumerable<Category>> GetListAsync(ArticlePlatform platform);
        Task<Category> GetAsync(ArticlePlatform platform, string shortName);

        Task<IEnumerable<Category>> GetListAsync();
        Task<IEnumerable<Category>> GetListAsync(ArticlePlatform platform, IEnumerable<string> shortNames);

        Task<IEnumerable<CategoryQueryDto>> GetChildrenAsync(ArticlePlatform platform, int parentId, List<int> includes = null);
        Task<IEnumerable<CategoryQueryDto>> GetChildrenAsync();
        Task<IEnumerable<CategoryQueryDto>> GetChildrenAsync(ArticlePlatform platform);
        Task<IEnumerable<CategoryQueryDto>> GetParentsAsync(ArticlePlatform platform, int childId, bool withself = false);
        Task<IEnumerable<Category>> GetChildrenFlatAsync(ArticlePlatform platform, int parentId);
        Task<IEnumerable<Category>> GetChildrenFlatWithselfAsync(ArticlePlatform platform, string shortName);
        Task<IEnumerable<Category>> GetChildrenFlatAsync(ArticlePlatform platform, List<int> parentIds, bool withself = false);
        Task UpdateShortNameAsync(IEnumerable<Category> entities);
        Task<Category> GetByNameAsync(ArticlePlatform platform, string name);
        Task<IEnumerable<Category>> GetChildrenFlatWithselfByNameAsync(ArticlePlatform platform, string name);
    }
}