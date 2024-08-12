using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.School.Common.OtherAPIClient.Article.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.Article
{
    public class ArticleAPIClient : IArticleAPIClient
    {
        readonly HttpClient _httpClient;
        public ArticleAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ArticleAPI");
        }

        public async Task<IEnumerable<GetRankingByEIDResponse>> GetRankingByEID(Guid eid)
        {
            if (eid == default) return null;
            var actionUrl = "Rank/GetRankingByEID/";
            var headers = new Dictionary<string, string>();
            var resp = await _httpClient.HttpPostJsonAsync<APIResult<IEnumerable<GetRankingByEIDResponse>>>(actionUrl, new { eid }, headers);
            if (resp?.Data?.Any() == true)
            {
                return resp.Data;
            }
            return null;
        }

        public async Task<IEnumerable<ListByEIDResponse>> ListByEID(Guid eid, bool containHTML = false)
        {
            if (eid == default) return null;

            var actionUrl = $"article/listbyeid/?eid={eid}&containHTML={containHTML}";
            var resp = await _httpClient.HttpPostAsync<APIResult<IEnumerable<ListByEIDResponse>>>(actionUrl);
            if (resp?.Data?.Any() == true)
            {
                return resp.Data;
            }
            return null;
        }

        public async Task<ListOrgLessonResponse> ListOrgLesson(ListOrgLessonRequest request)
        {
            var actionUrl = "RecommendOrgLesson/ListOrgLesson/";
            var resp = await _httpClient.HttpPostJsonAsync<APIResult<ListOrgLessonResponse>>(actionUrl, request);
            if (resp?.Data != default)
            {
                return resp.Data;
            }
            return null;
        }
    }
}
