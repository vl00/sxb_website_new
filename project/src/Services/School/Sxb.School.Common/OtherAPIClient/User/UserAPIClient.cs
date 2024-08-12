using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.School.Common.OtherAPIClient.User.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.User
{
    public partial class UserAPIClient : IUserAPIClient
    {
        readonly HttpClient _httpClient;
        public UserAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("UserAPI");
        }

        public async Task<bool> CheckIsCollected(Guid dataID, Guid userID)
        {
            if (dataID == default || userID == default) return false;
            var actionUrl = "Collection/CheckIsCollected/";
            var resp = await _httpClient.HttpPostJsonAsync<APIResult<dynamic>>(actionUrl, new { dataID, userID });
            if (resp?.Data != null)
            {
                return resp.Data.isCollected;
            }
            return false;
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

        public async Task<IEnumerable<Guid>> GetSubscribeRemindsUserIdsAsync(string groupCode, Guid subjectId, int pageIndex, int pageSize)
        {
            var actionUrl = $"SubscribeReminds/userIds/?groupCode={groupCode}&subjectId={subjectId}&pageIndex={pageIndex}&pageSize={pageSize}";
            var resp = await _httpClient.HttpGetAsync<APIResult<IEnumerable<Guid>>>(actionUrl);
            return resp?.Data ?? Enumerable.Empty<Guid>();
        }


        public async Task<bool> CheckIsSubscribe(string groupCode, Guid subjectId, Guid userId)
        {
            var actionUrl = $"SubscribeReminds/exists?groupCode={groupCode}&subjectId={subjectId}&userId={userId}";
            var resp = await _httpClient.HttpGetAsync<APIResult<SubscribeCheckDto>>(actionUrl);
            return resp?.Data?.IsSubscribe == true;
        }
    }
}
