using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sxb.WeWorkFinance.API.Application.Models;
using Newtonsoft.Json;
using System.Text;

namespace Sxb.WeWorkFinance.API.Application.HttpClients
{
    public class BgDataServerClient
    {
        private readonly HttpClient _httpClient;
        public BgDataServerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private class BackGroundRefundRequest
        {
            public Guid OrderDetailId { get; set; }
            public int RefundCount { get; set; } = 1;
        }

        public async Task<bool> BackGroundRefund(Guid orderDetailId, int backGroundRefund)
        {
            string url = "/Aftersales/BackGroundRefund";
            var request = new BackGroundRefundRequest
            {
                OrderDetailId = orderDetailId,
                RefundCount = backGroundRefund
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var resp = await _httpClient.PostAsync(url, content);

            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resultString = await resp.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<OrgResultModel<object>>(resultString);
                if (result.Succeed)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
