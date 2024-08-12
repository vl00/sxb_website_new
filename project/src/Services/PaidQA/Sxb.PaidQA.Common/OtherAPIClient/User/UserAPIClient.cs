using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.PaidQA.Common.OtherAPIClient.User.Model.Entity;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.PaidQA.Common.OtherAPIClient.User
{
    public class UserAPIClient : IUserAPIClient
    {
        readonly HttpClient _httpClient;
        public UserAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("UserAPI");
        }

        public async Task<Guid> GetExtensionTalentUserID(Guid eid)
        {
            if (eid == default) return default;
            var actionUrl = "Talent/GetExtensionTalentUserID/";
            var resp = await _httpClient.HttpPostJsonAsync<APIResult<string>>(actionUrl, new { eid });
            if (Guid.TryParse(resp?.Data, out Guid parseResult) == true)
            {
                return parseResult;
            }
            return default;
        }

        public async Task<TalentInfo> GetTalentDetail(Guid userID)
        {
            if (userID == default) return null;

            var actionUrl = "talent/GetByUserID/?userID=" + userID;
            var resp = await _httpClient.HttpPostAsync<APIResult<TalentInfo>>(actionUrl);
            if (resp?.Data?.ID != default)
            {
                return resp.Data;
            }
            return null;
        }
    }
}
