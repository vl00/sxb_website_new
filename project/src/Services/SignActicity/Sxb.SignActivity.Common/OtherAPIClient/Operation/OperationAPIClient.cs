using Microsoft.AspNetCore.JsonPatch.Operations;
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
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Common.OtherAPIClient.Marketing
{
    public class OperationAPIClient : IOperationAPIClient
    {
        readonly HttpClient _httpClient;
        readonly ILogger<OperationAPIClient> _logger;
        public OperationAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("OperationAPI");
        }

        public async Task<SMSAPIResult> SMSApi(string templateId, string[] phones, string[] templateParams)
        {
            var url = "api/SMSApi/SendSxbMessage";
            var data = new
            {
                templateId = templateId,
                phones = phones,
                templateParams = templateParams,
            };
            var resp = await PostAsync<SMSAPIResult>(url, data.ToJson());
            return resp;
        }


        /// <summary>
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public async Task<TResult> PostAsync<TResult>(string requestUri, string requestJson)
            where TResult : class
        {
            string body = string.Empty;
            try
            {
                using (var reqContent = new StringContent(requestJson, Encoding.UTF8, "application/json"))
                {
                    using (var resp = await _httpClient.PostAsync(requestUri, reqContent))
                    using (var respContent = resp.Content)
                    {

                        body = await respContent.ReadAsStringAsync();
                        return body.FromJson<TResult>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"请求运营后台失败。{body}");
                return default;
            }

        }
    }
}
