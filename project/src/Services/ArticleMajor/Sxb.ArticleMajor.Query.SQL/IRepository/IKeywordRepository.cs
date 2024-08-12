using Kogel.Dapper.Extension;
using Sxb.ArticleMajor.Common.Entity;
using Sxb.ArticleMajor.Common.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.SQL.IRepository
{
    public interface IKeywordRepository
    {
        Task<int> DeleteAsync(Guid id);
        Task<KeywordInfo> GetAsync(Guid id);
        Task<int> InsertAsync(KeywordInfo entity);
        Task<IEnumerable<KeywordInfo>> ListAsync(KeywordSiteType? siteType, int? cityCode, KeywordPageType? pageType, KeywordPositionType? positionType);
        Task<PageList<KeywordInfo>> PageAsync(int pageIndex = 1, int pageSize = 10);
        Task<int> UpdateAsync(KeywordInfo entity);
    }
}
