using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Framework.Foundation
{
    public static class HttpClientExt
    {
        public async static Task<T> ReadAsAsync<T>(this HttpContent content)
        {

            string responseStr = (await content.ReadAsStringAsync()).Trim();
            if (typeof(T) == typeof(string))
            {
                return (T)(object)responseStr;
            }
            else
            {
                try
                {
                    if (CommonHelper.IsJsonp(responseStr, out string jsonContent))
                    {
                        return JsonConvert.DeserializeObject<T>(jsonContent);
                    }
                    return JsonConvert.DeserializeObject<T>(responseStr);
                }
                catch (Exception)
                {
                    throw new Exception("返回数据解析失败");
                }
                //response.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                //return await response.Content.ReadAsAsync<T>();
            }
            //return default;


            //var contentString = await content.ReadAsStringAsync();

            //return JsonConvert.DeserializeObject<T>(contentString);


        }

        public async static Task<T> GetAsAsync<T>(this HttpClient client, string requestUrl, Dictionary<string, string> header = null)
        {
            var response = await client.GetAsync(requestUrl);
            if (header != null)
            {
                foreach (var item in header)
                {
                    response.Headers.Add(item.Key, item.Value);
                }
            }

            return await response.Content.ReadAsAsync<T>();
        }

        public async static Task<T> PostAsAsync<T>(this HttpClient client, string requestUrl, IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {

            var data = keyValuePairs
                .Select(kv => $"{kv.Key}={kv.Value.ToString()}");

            var content = new StringContent(string.Join("&", data), Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await client.PostAsync(requestUrl, content);

            return await response.Content.ReadAsAsync<T>();
        }
        public static async Task<T> HttpPostAsync<T>(this HttpClient client, string url, string postData = null, string contentType = null, Dictionary<string, string> headers = null)
        {
            postData = postData ?? "";
            if (headers != null)
            {
                foreach (var header in headers)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            using (HttpContent httpContent = new StringContent(postData, Encoding.UTF8))
            {
                if (contentType != null)
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, httpContent);
                    return await ReturnAsync<T>(response);
                }
                catch
                {
                }
            }

            return default;
        }
        public static async Task<T> HttpPostJsonAsync<T>(this HttpClient client, string url, object postData = null, Dictionary<string, string> headers = null)
        {
            return await HttpPostAsync<T>(client, url, postData.ToJson(), "application/json", headers);
        }

        public static async Task<string> HttpGetAsync(this HttpClient client, string url, Dictionary<string, string> headers = null)
        {
            return await HttpGetAsync<string>(client, url, headers);
        }
        public static async Task<T> HttpGetAsync<T>(this HttpClient client, string url, Dictionary<string, string> headers = null)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            HttpResponseMessage response = await client.GetAsync(url);
            return await ReturnAsync<T>(response);

        }

        public static async Task<T> ReturnAsync<T>(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responseStr = (await response.Content.ReadAsStringAsync()).Trim();
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)responseStr;
                }
                else
                {
                    try
                    {
                        if (CommonHelper.IsJsonp(responseStr, out string jsonContent))
                        {
                            return JsonConvert.DeserializeObject<T>(jsonContent);
                        }
                        return JsonConvert.DeserializeObject<T>(responseStr);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("返回数据解析失败", ex);
                    }
                    //response.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    //return await response.Content.ReadAsAsync<T>();
                }
            }
            return default;
        }
    }
}
