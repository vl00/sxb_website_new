using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.School.Common.DTO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.OrgLesson
{
    public class OrgLessonAPIClient : IOrgLessonAPIClient
    {
        readonly HttpClient _httpClient;
        public OrgLessonAPIClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("OrganizationAPI");
        }

        public async Task<OrgLessonDTO> GetOrgLesson(string apiUrl, byte grade)
        {
            if (string.IsNullOrWhiteSpace(apiUrl)) return null;
            var ageMin = 3;
            var ageMax = 6;
            switch (grade)
            {
                case 2:
                    ageMin = 6;
                    ageMax = 12;
                    break;
                case 3:
                    ageMin = 12;
                    ageMax = 15;
                    break;
                case 4:
                    ageMin = 0;
                    ageMax = 3;
                    break;
            }
            var resp = await _httpClient.HttpGetAsync<APIResult<OrgLessonDTO>>(string.Format(apiUrl, ageMin, ageMax));
            if (resp?.Data != null)
            {
                return resp.Data;
            }
            return null;
        }
    }
}
