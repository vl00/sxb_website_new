using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sxb.WeWorkFinance.API.Application.Models;
using Newtonsoft.Json;
using System.Text;

namespace Sxb.WeWorkFinance.API.Application.HttpClients
{
    public class MarketingServerClient
    {
        private readonly HttpClient _httpClient;
        public MarketingServerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private class PreLockFansRequest
        {
            public Guid FansUserId { get; set; }
            public Guid HeadUserId { get; set; }
            public int Channel { get; set; } = 1;
        }

        public async Task<bool> PreLockFans(Guid fansUserId, Guid headUserId)
        {
            string url = "/api/FxUser/PreLockFans";
            var request = new PreLockFansRequest {
                FansUserId = fansUserId,
                HeadUserId = headUserId
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var resp = await _httpClient.PostAsync(url, content);

            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resultString = await resp.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<OrgResultModel<object>>(resultString);
                if (result.Status == 200)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
