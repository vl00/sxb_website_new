using Sxb.ArticleMajor.Common.Entity;
using Sxb.ArticleMajor.Common.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.API.Application.Query
{
    public interface IKeywordQuery
    {
        Task<IEnumerable<KeywordInfo>> ListAsync(KeywordSiteType? siteType, int? cityCode, KeywordPageType? pageType, KeywordPositionType? positionType);
    }
}
