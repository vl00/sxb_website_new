using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.User
{
    public class VerifyCodeAPIClient : IVerifyCodeAPIClient
    {
        readonly HttpClient _httpClient;
        public VerifyCodeAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("UserAPI");
        }

        public async Task<bool> VerifyRndCodeAsync(string mobile, string codeType, string code, string nationCode = "86")
        {
            if (string.IsNullOrWhiteSpace(mobile) || string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(codeType) || string.IsNullOrWhiteSpace(nationCode)) return false;
            var actionUrl = "VerifyCode/VerifyRndCode/";
            var resp = await _httpClient.HttpPostJsonAsync<ResponseResult>(actionUrl, new { mobile, code, codeType, nationCode });
            if (resp != null)
            {
                return resp.Succeed;
            }
            return false;
        }
    }
}
