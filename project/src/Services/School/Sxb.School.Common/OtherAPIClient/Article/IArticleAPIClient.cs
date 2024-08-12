using Sxb.School.Common.OtherAPIClient.Article.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.Article
{
    public interface IArticleAPIClient
    {
        Task<IEnumerable<ListByEIDResponse>> ListByEID(Guid eid, bool containHTML = false);
        Task<ListOrgLessonResponse> ListOrgLesson(ListOrgLessonRequest request);
        Task<IEnumerable<GetRankingByEIDResponse>> GetRankingByEID(Guid eid);
    }
}
