using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.OtherAPIClient.User.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.OtherAPIClient.User
{
    public class UserApiService : IUserApiService
    {
        readonly Func<HttpClient> _clientFunc;
        readonly IConfiguration _config;

        public UserApiService(IHttpClientFactory factory, IConfiguration config)
        {
            this._config = config;

            _clientFunc = () => factory.CreateClient("OpenApi");
        }

        string GetBaseUrl()
        {
            return _config["ExternalInterface:Sxb-OpenApi"].TrimEnd('/') + "/user";
        }

        public async Task<bool> HasGzWxGzh(Guid userid)
        {
            using var http = _clientFunc();
            var url = $"{GetBaseUrl()}/Users/CheckIsSubscribe?userid={userid}";
            var res = await http.GetAsync(url);
            res.EnsureSuccessStatusCode();
            var r = (await res.Content.ReadAsStringAsync()).FromJson<APIResult<JToken>>();
            if (!r.Succeed) throw new ResponseResultException(r.Msg, (int)r.status);
            return ((bool?)r.Data["isSubscribe"]) ?? false;
        }

        public async Task<bool> IsRealUser(Guid userid)
        {
            using var http = _clientFunc();
            var url = $"{GetBaseUrl()}/Users/IsRealUser?userid={userid}";
            var res = await http.GetAsync(url);
            res.EnsureSuccessStatusCode();
            var r = (await res.Content.ReadAsStringAsync()).FromJson<APIResult<JToken>>();
            if (!r.Succeed) throw new ResponseResultException(r.Msg, (int)r.status);
            return ((bool?)r.Data["isReal"]) ?? false;
        }

        public async Task<bool> IsUserBindMobile(Guid userId)
        {
            using var http = _clientFunc();
            var url = $"{GetBaseUrl()}/Users/IsUserBindMobile?userid={userId}";
            var res = await http.GetAsync(url);
            res.EnsureSuccessStatusCode();
            var r = (await res.Content.ReadAsStringAsync()).FromJson<APIResult<JToken>>();
            if (!r.Succeed) throw new ResponseResultException(r.Msg, (int)r.status);
            return ((bool?)r.Data["isBind"]) ?? false;
        }

        public async Task<IEnumerable<UserDescDto>> GetUsersDesc(IEnumerable<Guid> userIds)
        {
            if (userIds?.Any() != true) return Enumerable.Empty<UserDescDto>();
            using var http = _clientFunc();
            var url = $"{GetBaseUrl()}/Users/GetUsersDesc";
            var res = await http.PostAsync(url, new StringContent(userIds.ToJson(), Encoding.UTF8, "application/json"));
            res.EnsureSuccessStatusCode();
            var r = (await res.Content.ReadAsStringAsync()).FromJson<APIResult<JToken>>();
            if (!r.Succeed) throw new ResponseResultException(r.Msg, (int)r.status);
            return r.Data["items"]?.ToString().FromJson<UserDescDto[]>();
        }

        public async Task<UserWxUnionIdDto> GetUserWxUnionId(string id)
        {
            using var http = _clientFunc();
            var url = $"{GetBaseUrl()}/Users/wx/unionid?id={id}";
            var res = await http.GetAsync(url);
            res.EnsureSuccessStatusCode();
            var r = (await res.Content.ReadAsStringAsync()).FromJson<APIResult<UserWxUnionIdDto>>();
            if (!r.Succeed) throw new ResponseResultException(r.Msg, (int)r.status);
            return r.Data;
        }

        public async Task<IEnumerable<TalentUserDescDto>> GetTopNTalentUserByGrade(int grade, int top)
        {
            using var http = _clientFunc();
            var url = $"{GetBaseUrl()}/Users/GetTopNTalentUserByGrade?grade={grade}&top={top}";
            var res = await http.GetAsync(url);
            res.EnsureSuccessStatusCode();
            var r = (await res.Content.ReadAsStringAsync()).FromJson<APIResult<JToken>>();
            if (!r.Succeed) throw new ResponseResultException(r.Msg, (int)r.status);
            return r.Data["data"]?.ToString().FromJson<TalentUserDescDto[]>();
        }

        public async Task<IEnumerable<UserDescDto>> GetTopNRandVirtualUser(int top)
        {
            using var http = _clientFunc();
            var url = $"{GetBaseUrl()}/Users/GetTopNRandVirtualUser?top={top}";
            var res = await http.GetAsync(url);
            res.EnsureSuccessStatusCode();
            var r = (await res.Content.ReadAsStringAsync()).FromJson<APIResult<JToken>>();
            if (!r.Succeed) throw new ResponseResultException(r.Msg, (int)r.status);
            return r.Data["data"]?.ToString().FromJson<UserDescDto[]>();
        }


        public async Task<IEnumerable<UserWxFwhDto>> GetWxNicknames(IEnumerable<Guid> ids)
        {
            var url = $"{GetBaseUrl()}/Users/WxNicknames";
            var res = await _clientFunc().PostAsJsonAsync(url, ids);
            res.EnsureSuccessStatusCode();
            var r = await res.Content.ReadAsAsync<APIResult<IEnumerable<UserWxFwhDto>>>();

            return r?.Data ?? Enumerable.Empty<UserWxFwhDto>();
        }
    }
}
