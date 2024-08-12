using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter.Model;
using Sxb.SignActivity.Common.OtherAPIClient;
using Sxb.SignActivity.Common.OtherAPIClient.Marketing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.OtherAPIClient.Marketing
{
    public class MarketingAPIClient : IMarketingAPIClient
    {
        readonly HttpClient _httpClient;
        readonly ILogger<MarketingAPIClient> _logger;
        public MarketingAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MarketingAPI");
        }

        public async Task<FxUserResponse> GetFxUser(Guid userId)
        {
            string url = $"/api/FxUser/Get?userId={userId}";
            var httpResponseMessage = await _httpClient.GetAsync(url);
            var resp = await httpResponseMessage.Content.ReadAsAsync<APIResult<FxUserResponse>>();
            return resp.Data;
        }


        public async Task<PreLockFxUserResponse> GetPreLockFxUser(Guid userId)
        {
            string url = $"/api/FxUser/GetLockFansInfoInside";
            var postData = new { fansUserId = userId }.ToJson();
            var resp = await _httpClient.PostAsync<APIResult<PreLockFxUserResponse>>(url, postData);
            return resp.Data;
        }


        public async Task<UserWxOpenInfo> GetWxOpenInfo(Guid userId)
        {
            string url = $"/api/Withdraw/GetWxOpenInfo?userId=" + userId;
            var httpResponseMessage = await _httpClient.GetAsync(url);
            var resp = await httpResponseMessage.Content.ReadAsAsync<APIResult<UserWxOpenInfo>>();
            return resp.Data;
        }

        public async Task<IEnumerable<Guid>> GetConsultantUserIds()
        {
            string url = $"/api/FxUser/GetAllUserIds?roles[0]=2&roles[1]=3";
            var httpResponseMessage = await _httpClient.GetAsync(url);
            var resp = await httpResponseMessage.Content.ReadAsAsync<APIResult<IEnumerable<Guid>>>();
            return resp.Data ?? Enumerable.Empty<Guid>();
        }
    }
}
