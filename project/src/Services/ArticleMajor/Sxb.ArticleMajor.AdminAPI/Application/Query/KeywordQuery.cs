using Kogel.Dapper.Extension;
using Sxb.ArticleMajor.Common.Entity;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Query.SQL.IRepository;

namespace Sxb.ArticleMajor.AdminAPI.Application.Query
{
    public class KeywordQuery : IKeywordQuery
    {
        readonly IKeywordRepository _keywordRepository;
        public KeywordQuery(IKeywordRepository keywordRepository)
        {
            _keywordRepository = keywordRepository;
        }

        public async Task<IEnumerable<KeywordInfo>> ListAsync(KeywordSiteType? siteType, int? cityCode, KeywordPageType? pageType, KeywordPositionType? positionType)
        {
            return await _keywordRepository.ListAsync(siteType, cityCode, pageType, positionType);
        }

        public async Task<KeywordInfo> GetAysnc(Guid id)
        {
            if (id == default) return default;
            return await _keywordRepository.GetAsync(id);
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            if (id == default) return default;
            return await _keywordRepository.DeleteAsync(id) > 0;
        }

        public async Task<bool> SaveAsync(KeywordInfo entity)
        {
            if (entity == default) return default;
            if (entity.ID == default) return await _keywordRepository.InsertAsync(entity) > 0;
            return await _keywordRepository.UpdateAsync(entity) > 0;
        }

        public async Task<PageList<KeywordInfo>> PageAsync(int pageIndex = 1, int pageSize = 10)
        {
            if (pageIndex < 1 || pageSize < 1) return default;
            return await _keywordRepository.PageAsync(pageIndex, pageSize);
        }
    }
}
