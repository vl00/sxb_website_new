using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.AspNetCoreHelper.Utils;
using Sxb.Framework.Foundation;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.OtherAPIClient.Org
{
    public class OrgApiService : IOrgApiService
    {
        readonly Func<HttpClient> _clientFunc;
        readonly IConfiguration _config;

        public OrgApiService(IHttpClientFactory factory, IConfiguration config)
        {
            this._config = config;

            _clientFunc = () => factory.CreateClient("OrgAPI");
        }

        string GetBaseUrl()
        {
            return _config["ExternalInterface:OrganizationAddress"].TrimEnd('/');
        }

        public async Task DelRedisKeys(IEnumerable<string> keys, int waitSec = 2)
        {
            var t1 = DelRedisKeys_Core(keys);
            var t2 = Task.Delay(TimeSpan.FromSeconds(waitSec));
            await Task.WhenAny(t1, t2);
        }

        async Task DelRedisKeys_Core(IEnumerable<string> keys)
        {
            //await Task.Delay(1000 * 10);
            var serviceScopeFactory = HttpContextModel.GetServiceScopeFactory();
            using var serviceScope = serviceScopeFactory.CreateScope();
            var services = serviceScope.ServiceProvider;
            try
            {                
                using var http = services.GetService<IHttpClientFactory>().CreateClient("OrgAPI");
                var url = $"{GetBaseUrl()}/api/test/delrediskeys";
                var res = await http.PostAsync(url, new StringContent(keys.ToJson(), Encoding.UTF8, "application/json"));
                res.EnsureSuccessStatusCode();
                var r = (await res.Content.ReadAsStringAsync()).FromJson<APIResult<JToken>>();
                if (!r.Succeed) throw new ResponseResultException(r.Msg, (int)r.status);
            }
            catch { }
        }        



    }
}
