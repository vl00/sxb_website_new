using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter
{
    public class FinanceCenterAPIClient : IFinanceCenterAPIClient
    {
        readonly HttpClient _httpClient;
        readonly ILogger<FinanceCenterAPIClient> _logger;
        public FinanceCenterAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FinanceCenterAPI");
        }

        public async Task<APIResult<WalletResponse>> FreezeAmountIncome(WalletRequest request)
        {
            string url = "/api/Wallet/FreezeAmountIncome";
            string postData = request.ToJson();
            var resp = await FinancialPostAsync<WalletResponse>(url, postData);
            return resp;
        }

        public async Task<APIResult<string>> InsideUnFreezeAmount(string freezeMoneyInLogId)
        {
            string url = "/api/Wallet/InsideUnFreezeAmount";
            string postData = new { freezeMoneyInLogId }.ToJson();
            var resp = await FinancialPostAsync<string>(url, postData);
            return resp;
        }

        /// <summary>
        /// 请求支付中心接口统一调用的httpclient，里面会代签名
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public async Task<APIResult<TResult>> FinancialPostAsync<TResult>(string requestUri, string requestJson)
            where TResult :class
        {
            string body = string.Empty;
            try
            {
                using (var reqContent = new StringContent(requestJson, Encoding.UTF8, "application/json"))
                {
                    var timespan = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var nonce = Guid.NewGuid().ToString("N");
                    reqContent.Headers.Add("sxb.timespan", timespan);
                    reqContent.Headers.Add("sxb.nonce", nonce);
                    reqContent.Headers.Add("sxb.key", "AskSystem");
                    StringBuilder query = new StringBuilder("woaisxb2021");//私钥，不能泄露
                    query.Append($"{timespan}\n{nonce}\n{requestJson}\n");
                    MD5 md5 = MD5.Create();
                    byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));
                    StringBuilder result = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        result.Append(bytes[i].ToString("X2"));
                    }
                    var sign = result.ToString();
                    reqContent.Headers.Add("sxb.sign", sign);
                    using (var resp = await _httpClient.PostAsync(requestUri, reqContent))
                    using (var respContent = resp.Content)
                    {

                        body = await respContent.ReadAsStringAsync();
                        
                        return body.FromJson<APIResult<TResult>>();
                    }
                }



            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"请求支付中心失败。{body}");
                return new APIResult<TResult>() { Succeed = false, Msg = $"请求支付中心失败,{ex.Message}" };
            }

        }

    }
}
