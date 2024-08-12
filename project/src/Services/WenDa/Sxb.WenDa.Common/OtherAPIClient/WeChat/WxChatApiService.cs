using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Sxb.WenDa.Common.OtherAPIClient.WeChat
{
    public class WxChatApiService : IWxChatApiService
    {
        readonly HttpClient _httpClient;
        readonly ILogger<WxChatApiService> _logger;
        public WxChatApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WxChatAPI");
        }

        /// <summary>
        /// 获取微信服务号的AccessToken
        /// api/accesstoken/gettoken?app=fwh
        /// </summary>
        /// <returns></returns>
        public async Task<WxApiResult<string>> GetSenceQRCode(WPScanRequestData requestData)
        {
            string url = "api/QRCode/GetSenceQRCode";
            var res = await _httpClient.PostAsJsonAsync(url, requestData);
            res.EnsureSuccessStatusCode();
            var r = await res.Content.ReadFromJsonAsync<WxApiResult<string>>();
            return r;
        }

        /// <summary>
        /// 发送客服消息
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<WxApiResult<object>> SendNewsMsg(NewsCustomMsg requestData)
        {
            string url = "api/Message/SendNewsMsg?app=0";
            var res = await _httpClient.PostAsJsonAsync(url, requestData);
            res.EnsureSuccessStatusCode();
            var r = await res.Content.ReadFromJsonAsync<WxApiResult<object>>();
            return r;
        }

    }
}
