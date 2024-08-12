using Sxb.School.API.Application.Query.ViewModel;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface IOrgLessonQuery
    {
        Task<OrgLessonViewModel> GetOrgLesson(string _apiUrl, byte grade);
    }
}
