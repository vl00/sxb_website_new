using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.School.API.Infrastructures.Services
{

    public class UserCenterService : IUserCenterService
    {
        HttpClient _client;
        public UserCenterService(IHttpClientFactory factory, IConfiguration configuration)
        {
            string baseurl = configuration["ExternalInterface:UserCenterAddress"];
            _client = factory.CreateClient("InnerClient");
            _client.BaseAddress = new System.Uri(baseurl);
        }

        public async Task<string> GetLoginCodeAsync(Guid userId)
        {
            string rawurl = $"/ApiLogin/GetLoginCode?userId={userId}";
            var response = await _client.GetAsync(rawurl);
            response.EnsureSuccessStatusCode();
            var ret = await response.Content.ReadAsStringAsync();
            var ret_jobj = JObject.Parse(ret);
            if (ret_jobj["succeed"].Value<bool>())
            {
                return ret_jobj["data"].Value<string>();
            }
            else
            {
                throw new Exception(ret_jobj["msg"].Value<string>());
            }

        }
    }
}
