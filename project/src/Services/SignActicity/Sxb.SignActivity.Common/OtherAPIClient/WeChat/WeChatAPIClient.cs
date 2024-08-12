using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter.Model;
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
    public class WeChatAPIClient : IWeChatAPIClient
    {
        readonly HttpClient _httpClient;
        readonly ILogger<WeChatAPIClient> _logger;
        public WeChatAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WxChatAPI");
        }


        /// <summary>
        /// 获取微信服务号的AccessToken
        /// api/accesstoken/gettoken?app=fwh
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAccessToken()
        {

            string url = "api/accesstoken/gettoken?app=fwh";

            var resp = await _httpClient.GetAsync(url);

            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resultString = await resp.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<AccessTokenResultModel>(resultString);
                if (result.Success)
                {
                    return result.Data?.Token;
                }
            }
            return null;
        }



    }
}
