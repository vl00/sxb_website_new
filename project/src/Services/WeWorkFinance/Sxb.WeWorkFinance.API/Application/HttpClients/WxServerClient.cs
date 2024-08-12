using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sxb.WeWorkFinance.API.Application.Models;
using Newtonsoft.Json;

namespace Sxb.WeWorkFinance.API.Application.HttpClients
{
    public class WxServerClient
    {
        private readonly HttpClient _httpClient;
        public WxServerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
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

            if(resp.StatusCode == System.Net.HttpStatusCode.OK )
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
