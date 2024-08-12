using Sxb.ArticleMajor.Common.Entity;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Query.SQL.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.API.Application.Query
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
    }
}
