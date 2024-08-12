using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.WxWork
{
    public class WxWorkAPIClient : IWxWorkAPIClient
    {
        readonly HttpClient _httpClient;
        public WxWorkAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WxWorkAPI");
        }
        public async Task<bool> SendWxWorkMsgAsync(IEnumerable<string> userIDs, string content, string agentID)
        {
            if (userIDs == default || !userIDs.Any() || string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(agentID)) return false;
            var apiUrl = "interface/sendweworkmsg/";
            var resp = await _httpClient.HttpPostJsonAsync<ResponseResult>(apiUrl, new { userID = userIDs, Msg = content, agentID });
            if (resp != null) return resp.Succeed;
            return false;
        }
    }
}
