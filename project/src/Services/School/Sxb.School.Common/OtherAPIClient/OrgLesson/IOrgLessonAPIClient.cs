using Sxb.School.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.OrgLesson
{
    public interface IOrgLessonAPIClient
    {
        Task<OrgLessonDTO> GetOrgLesson(string apiUrl, byte grade);
    }
}
