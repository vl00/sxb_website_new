using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Sxb.Framework.Foundation;

namespace Sxb.SignActivity.Common.OtherAPIClient
{
    internal static class HttpClientExtension
    {

        /// <summary>
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public static async Task<TResult> PostAsync<TResult>(this HttpClient httpClient, string requestUri, string requestJson)
            where TResult : class
        {
            string body = string.Empty;
            using (var reqContent = new StringContent(requestJson, Encoding.UTF8, "application/json"))
            {
                using (var resp = await httpClient.PostAsync(requestUri, reqContent))
                using (var respContent = resp.Content)
                {

                    body = await respContent.ReadAsStringAsync();
                    return body.FromJson<TResult>();
                }
            }
        }
    }
}
