using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.School.Common.OtherAPIClient.Comment.Model.Entity;
using Sxb.School.Common.OtherAPIClient.Comment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.Comment
{
    public class CommentAPIClient : ICommentAPIClient
    {
        readonly HttpClient _httpClient;
        public CommentAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CommentAPI");
        }

        public async Task<IEnumerable<SchoolCommentTotalDTO>> GetCommentTotals(Guid sid)
        {
            if (sid == default) return null;
            var actionUrl = "Comment/GetCommentTotal/";
            var resp = await _httpClient.HttpPostJsonAsync<APIResult<IEnumerable<SchoolCommentTotalDTO>>>(actionUrl, new { sid });
            if (resp?.Data?.Any() == true)
            {
                return resp.Data;
            }
            return null;
        }

        public async Task<IEnumerable<GetQuestionByEIDResponse>> GetQuestionByEID(Guid eid, Guid userID,int take =1)
        {
            if (eid == default) return null;

            var req = new GetQuestionByEIDRequest()
            {
                EID = eid,
                UserID = userID
            };
            var actionUrl = $"question/get/?take={take}&" + req.QueryString;
            var resp = await _httpClient.HttpPostAsync<APIResult<IEnumerable<GetQuestionByEIDResponse>>>(actionUrl);
            if (resp?.Data?.Any() == true)
            {
                return resp.Data;
            }
            return null;
        }

        public async Task<IEnumerable<SchoolQuestionTotalDTO>> GetQuestionTotals(Guid eid)
        {
            if (eid == default) return null;
            var actionUrl = "Question/ListQuestionTotals/";
            var resp = await _httpClient.HttpPostJsonAsync<APIResult<IEnumerable<SchoolQuestionTotalDTO>>>(actionUrl, new { eid });
            if (resp?.Data?.Any() == true)
            {
                return resp.Data;
            }
            return null;
        }

        public async Task<IEnumerable<SchoolCommentDTO>> GetSchoolSelectedComment(Guid eid, Guid? userID, int take = 1)
        {
            if (eid == default) return null;

            var actionUrl = "comment/GetSchoolSelectedComment/";
            var resp = await _httpClient.HttpPostJsonAsync<APIResult<IEnumerable<SchoolCommentDTO>>>(actionUrl, new { eid, userID, take });
            if (resp?.Data != default)
            {
                return resp.Data;
            }
            return null;
        }

        public async Task<IEnumerable<SchoolScoreCommentCountDto>> GetSchoolScoreCommentCountByEids(IEnumerable<Guid> eids)
        {
            if ((eids?.Count() ?? 0) < 1) return null;
            var url = "comment/GetSchoolScoreCommentCountByEids";
            var resp = await _httpClient.HttpPostJsonAsync<APIResult<IEnumerable<SchoolScoreCommentCountDto>>>(url, new { eids });
            if (resp.Succeed && resp?.Data != default)
            {
                return resp.Data;
            }
            return null;
        }
    }
}
