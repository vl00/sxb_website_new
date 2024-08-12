using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace Sxb.Settlement.API.Services
{
    public class HttpCallBackNotifyService : IHttpCallBackNotifyService
    {
        HttpClient _client;
        ILogger<HttpCallBackNotifyService> _logger;
        public HttpCallBackNotifyService(HttpClient client
            , ILogger<HttpCallBackNotifyService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task NotifySettlementRefundSuccess(string url, SettlementRefundSuccessMessage settlementRefundMessage)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url不能为空");
            }
            string requestBody = JsonConvert.SerializeObject(settlementRefundMessage);
            HttpContent httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            httpContent.Headers.TryAddWithoutValidation("RequestId", Guid.NewGuid().ToString());
            httpContent.Headers.TryAddWithoutValidation("App", "Sxb.Settlement.API");
            var response = await _client.PostAsync(url, httpContent);
            string responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(responseBody);
        }

        public async Task NotifySettlementStatus(string url, SettlementStatusMessage settlementStatusMessage)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url不能为空");
            }
            string requestBody = JsonConvert.SerializeObject(settlementStatusMessage, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            HttpContent httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            httpContent.Headers.TryAddWithoutValidation("RequestId", Guid.NewGuid().ToString());
            httpContent.Headers.TryAddWithoutValidation("App", "Sxb.Settlement.API");
            var response = await _client.PostAsync(url, httpContent);
            string responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(responseBody);
        }
    }
}
