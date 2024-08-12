using Sxb.Comment.Common.OtherAPIClient.User.Model.Entity;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.Comment.Common.OtherAPIClient.User
{
    public class UserAPIClient : IUserAPIClient
    {
        readonly HttpClient _httpClient;
        public UserAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("UserAPI");
        }

        public async Task<TalentInfo> GetTalentDetail(Guid userID)
        {
            if (userID == default) return null;

            var actionUrl = "talent/GetByUserID/?userID=" + userID;
            var resp = await _httpClient.HttpPostAsync<APIResult<TalentInfo>>(actionUrl);
            if (resp?.Data?.ID != default)
            {
                return resp.Data;
            }
            return null;
        }

        public async Task<IEnumerable<TalentInfo>> ListTalentDetails(IEnumerable<Guid> userIDs)
        {
            if (userIDs?.Any() == true)
            {

                var actionUrl = "talent/ListByUserIDs/";
                var resp = await _httpClient.HttpPostJsonAsync<APIResult<IEnumerable<TalentInfo>>>(actionUrl, new { userIDs });
                if (resp?.Data?.Any() == true)
                {
                    return resp.Data;
                }
            }
            return null;
        }
    }
}
