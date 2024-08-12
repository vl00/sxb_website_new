using Microsoft.Extensions.Configuration;
using Sxb.School.API.Infrastructures.Services.Models;
using Sxb.School.API.Utils;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.API.Infrastructures.Services
{
    public class PayGatewayService : IPayGatewayService
    {
        HttpClient _client;
        public PayGatewayService(IHttpClientFactory factory, IConfiguration configuration)
        {
            string baseurl = configuration["ExternalInterface:PayGatewayAddress"];
            _client = factory.CreateClient("InnerClient");
            _client.BaseAddress = new System.Uri(baseurl);


        }

        public async Task<(string orderNo,string orderId)> PayByH5(AddPayOrderRequest request)
        {
            string rawurl = "/api/PayOrder/Add";
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
            content = PayRequestHelper.SetSignHeader(content, Configs.PayKey, body, Configs.PaySystem);
            var response = await _client.PostAsync(rawurl, content);
            response.EnsureSuccessStatusCode();
            var ret = await response.Content.ReadAsStringAsync();
            var ret_response = Newtonsoft.Json.JsonConvert.DeserializeObject<PayGatewayBaseResponse<PayByH5Response>>(ret);
            if (ret_response.succeed)
                return (ret_response.data.orderno,ret_response.data.orderid);
            else
                throw new Exception(ret_response.msg);
        }

     

    }
}
