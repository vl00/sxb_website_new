using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.OtherAPIClient.WxWork.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.OtherAPIClient.WxWork
{
    public class WxWorkApiService : IWxWorkApiService
    {
        readonly Func<HttpClient> _clientFunc;
        readonly IConfiguration _config;

        public WxWorkApiService(IHttpClientFactory factory, IConfiguration config)
        {
            this._config = config;

            _clientFunc = () => factory.CreateClient("WxWorkApi");
        }

        string GetBaseUrl()
        {
            return _config["ExternalInterface:WxWorkApi"].TrimEnd('/');
        }

        public async Task<GetAddCustomerQrCodeResDto> GetAddCustomerQrCode(GetAddCustomerQrCodeReqDto dto)
        {
            using var http = _clientFunc();
            var url = $"{GetBaseUrl()}/api/SchoolMoreDetail/GetAddCustomerQrCode";
            var res = await http.PostAsync(url, new StringContent(dto.ToJson(), Encoding.UTF8, "application/json"));
            res.EnsureSuccessStatusCode();
            var r = (await res.Content.ReadAsStringAsync()).FromJson<APIResult<GetAddCustomerQrCodeResDto>>();
            if (!r.Succeed) throw new ResponseResultException(r.Msg, (int)r.status);
            return r.Data;
        }

    }
}
