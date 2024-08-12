using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.API.Infrastructure.Services.User.Model;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Sxb.Framework.Foundation;

namespace Sxb.PointsMall.API.Infrastructure.Services
{
    public class UserService : IUserService
    {
        readonly HttpClient _httpClient;
        public UserService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("UserAPI");
        }

        public async Task<bool> GetUserSubscribe(Guid userId)
        {
            if (userId == default) return default;

            var actionUrl = "Users/CheckIsSubscribe?userId=" + userId;
            var resp = await _httpClient.HttpGetAsync<APIResult<SubscribeInfo>>(actionUrl);
            return resp?.Data?.IsSubscribe == true;
        }
    }
}
