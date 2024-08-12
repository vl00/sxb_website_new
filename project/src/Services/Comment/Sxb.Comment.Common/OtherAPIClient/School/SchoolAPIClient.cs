using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.Comment.Common.OtherAPIClient.School
{
    public class SchoolAPIClient : ISchoolAPIClient
    {
        readonly HttpClient _httpClient;
        public SchoolAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SchoolAPI");
        }

        public async Task<IEnumerable<Guid>> GetValidEIDs(Guid sid)
        {
            if (sid == default) return default;
            var apiUrl = $"school/GetValidEIDs/?sid={sid}";
            var resp = await _httpClient.HttpGetAsync<APIResult<IEnumerable<Guid>>>(apiUrl);
            if (resp?.Data != default) return resp.Data;
            return default;
        }
    }
}
