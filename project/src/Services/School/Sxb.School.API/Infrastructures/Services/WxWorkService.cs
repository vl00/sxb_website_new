using Microsoft.Extensions.Configuration;
using Sxb.School.API.Infrastructures.Services.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.API.Infrastructures.Services
{
    public class WxWorkService: IWxWorkService
    {
        HttpClient _client;

        public WxWorkService(IHttpClientFactory factory, IConfiguration configuration)
        {
            string baseurl = configuration["ExternalInterface:WxWorkAddress"];
            _client = factory.CreateClient("InnerClient");
            _client.BaseAddress = new Uri(baseurl);
        }

        public async  Task<GetAddCustomerQrCodeResponse> GetAddCustomerQrCode(GetAddCustomerQrCodeRequest request)
        {
            string rawurl = "/api/SchoolMoreDetail/GetAddCustomerQrCode";
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(rawurl, content);
            response.EnsureSuccessStatusCode();
            var ret = await response.Content.ReadAsStringAsync();
            var ret_response = Newtonsoft.Json.JsonConvert.DeserializeObject<WxWorkBaseResponse<GetAddCustomerQrCodeResponse>>(ret);
            if (ret_response.succeed)
                return ret_response.data;
            else
                throw new Exception(ret_response.msg);
        }
    }
}
