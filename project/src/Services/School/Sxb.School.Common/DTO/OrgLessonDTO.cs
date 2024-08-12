using Sxb.School.Common.Entity;
using System.Collections.Generic;

namespace Sxb.School.Common.DTO
{
    public class OrgLessonDTO
    {
        public IEnumerable<RecommendOrgInfo> RecommendOrgs { get; set; }
        public IEnumerable<HotSellCourseInfo> HotSellCourses { get; set; }
    }
}
