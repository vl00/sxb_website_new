using Microsoft.Extensions.Configuration;
using Sxb.School.API.Infrastructures.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.API.Infrastructures.Services
{
    public class WeChatGatewayService : IWeChatGatewayService
    {
        HttpClient _client;
        public WeChatGatewayService(IHttpClientFactory factory, IConfiguration configuration)
        {
            string baseurl = configuration["ExternalInterface:WxGatewayAddress"];
            _client = factory.CreateClient("InnerClient");
            _client.BaseAddress = new Uri(baseurl);
        }

        public async Task<string> GetSenceQRCode(GetSenceQRCodeRequest request)
        {
            string rawurl = "/api/QRCode/GetSenceQRCode";
            HttpContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(rawurl, content);
            response.EnsureSuccessStatusCode();
            var ret = await response.Content.ReadAsStringAsync();
            var ret_response = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChatGatewayBaseResponse<string>>(ret);
            if (ret_response.success)
                return ret_response.data;
            else
                throw new Exception(ret_response.msg);
        }

        public async Task SendSendTextMsg(string toUser, string content)
        {
            string rawurl = "/api/Message/SendTextMsg?app=fwh";
            HttpContent httpcontent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new { toUser, content }), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(rawurl, httpcontent);
            response.EnsureSuccessStatusCode();
            var ret = await response.Content.ReadAsStringAsync();
            var ret_response = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChatGatewayBaseResponse<string>>(ret);
            if (!ret_response.success)
                throw new Exception(ret_response.msg);
        }

        public async Task SendNewsMsg(string toUser, string title, string description, string url, string picUrl)
        {
            string rawurl = "/api/Message/SendNewsMsg?app=fwh";
            HttpContent httpcontent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new { toUser, title, description, url, picUrl }), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(rawurl, httpcontent);
            response.EnsureSuccessStatusCode();
            var ret = await response.Content.ReadAsStringAsync();
            var ret_response = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChatGatewayBaseResponse<string>>(ret);
            if (!ret_response.success)
                throw new Exception(ret_response.msg);
        }
        public async Task SendImgMsg(string toUser, Stream file)
        {


            string rawurl = "/api/Message/SendImgMsg?app=fwh";
            var formData = new MultipartFormDataContent();
            /*----------------------------------------*/
            formData.Add(new StringContent(toUser), "toUser");
            /*----------------------------------------*/
            file.Seek(0, SeekOrigin.Begin);
            var streamContent = new StreamContent(file);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            formData.Add(streamContent, "file", $"{Guid.NewGuid()}.jpeg");
            /*----------------------------------------*/
            var response = await _client.PostAsync(rawurl, formData);
            response.EnsureSuccessStatusCode();
            var ret = await response.Content.ReadAsStringAsync();
            var ret_response = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChatGatewayBaseResponse<string>>(ret);
            if (!ret_response.success)
                throw new Exception(ret_response.msg);
        }

        public async Task SendTplMsg(string toUser, string tplId, string url, List<TplDataFiled> fileds)
        {
            string rawurl = "/api/Message/SendTplMsg?app=fwh";
            HttpContent httpcontent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                toUser = toUser,
                templateId = tplId,
                url = url,
                fileds = fileds

            }), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(rawurl, httpcontent);
            response.EnsureSuccessStatusCode();
            var ret = await response.Content.ReadAsStringAsync();
            var ret_response = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChatGatewayBaseResponse<string>>(ret);
            if (!ret_response.success)
                throw new Exception(ret_response.msg);
        }
    }
}
