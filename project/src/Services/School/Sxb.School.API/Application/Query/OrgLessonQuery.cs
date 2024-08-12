using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.School.API.Application.Query.ViewModel;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class OrgLessonQuery : IOrgLessonQuery
    {
        public async Task<OrgLessonViewModel> GetOrgLesson(string _apiUrl, byte grade)
        {
            if (string.IsNullOrWhiteSpace(_apiUrl)) return null;
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
            try
            {
                var webClient = new WebUtils();
                var apiResponse = await Task.Run(() =>
                {
                    return webClient.DoGet(string.Format(_apiUrl, ageMin, ageMax));
                });
                if (!string.IsNullOrWhiteSpace(apiResponse))
                {

                    var object_Response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseResult<OrgLessonViewModel>>(apiResponse);

                    if (object_Response.Succeed)
                    {
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<OrgLessonViewModel>(Newtonsoft.Json.JsonConvert.SerializeObject(object_Response.Data));
                    }
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
