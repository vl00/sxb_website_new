using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.School.Common.OtherAPIClient.PaidQA.Model.EntityExtend;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.PaidQA
{
    public class PaidQAAPIClient : IPaidQAAPIClient
    {
        readonly HttpClient _httpClient;
        public PaidQAAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PaidQAAPI");
        }

        public async Task<TalentDetailExtend> GetTalentByUserID(Guid userID)
        {
            if (userID == default) return null;
            var apiUrl = "talent/getbyuserid/?userid=" + userID;
            var resp = await _httpClient.HttpPostAsync<APIResult<TalentDetailExtend>>(apiUrl);
            if (resp?.Data != default) return resp.Data;
            return null;

        }

        public async Task<TalentDetailExtend> RandomTalentByGrade(int grade, bool isInternal = false, Guid? eid = null)
        {
            if (grade < 1) return null;
            var apiUrl = $"talent/RandomByGrade/";
            var resp = await _httpClient.HttpPostJsonAsync<APIResult<TalentDetailExtend>>(apiUrl, new { grade, isInternal, eid });
            if (resp?.Data != default) return resp.Data;
            return null;
        }
    }
}
