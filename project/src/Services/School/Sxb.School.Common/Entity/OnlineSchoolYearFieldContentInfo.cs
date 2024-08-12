using System;

namespace Sxb.School.Common.Entity
{
    public class OnlineSchoolYearFieldContentInfo
    {
        public Guid ID { get; set; }
        public int Year { get; set; }
        public Guid EID { get; set; }
        public string Field { get; set; }
        public string Content { get; set; }
        public bool IsValid { get; set; }
    }
}
