using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.ResponseDto.School;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.OtherAPIClient.School
{
    public class SchoolApiService : ISchoolApiService
    {
        readonly Func<HttpClient> _clientFunc;
        readonly IConfiguration _config;

        public SchoolApiService(IHttpClientFactory factory, IConfiguration config)
        {
            this._config = config;

            _clientFunc = () => factory.CreateClient("Sxb-OpenApi");
        }

        string GetBaseUrl()
        {
            return _config["ExternalInterface:Sxb-OpenApi"].TrimEnd('/') + "/school";
        }

        public async Task<List<SchoolIdAndNameDto>> GetSchoolsIdAndName(IEnumerable<string> eids)
        {
            using var http = _clientFunc();
            var url = $"{GetBaseUrl()}/CallSchoolApi/GetSchoolsIdAndName";
            var res = await http.PostAsync(url, new StringContent(eids.ToJson(), Encoding.UTF8, "application/json"));
            res.EnsureSuccessStatusCode();
            var r = (await res.Content.ReadAsStringAsync()).FromJson<APIResult<JToken>>();
            if (!r.Succeed) throw new ResponseResultException(r.Msg, (int)r.status);
            return r.Data["items"]?.ToString()?.FromJson<List<SchoolIdAndNameDto>>() ?? new List<SchoolIdAndNameDto>();
        }

        public async Task<List<SchoolIdAndNameDto>> GetSchoolsIdAndName(IEnumerable<Guid> eids)
        {
            return await GetSchoolsIdAndName(eids.Select(s=>s.ToString()));
        }

    }
}
