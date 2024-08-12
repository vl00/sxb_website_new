using System;

namespace Sxb.School.Common.OtherAPIClient.Comment.Model.Entity
{
    public class SchoolCommentTotalDTO
    {
        public Guid? SchoolSectionID { get; set; }
        public int Total { get; set; }
        public string TotalTypeName { get; set; }
    }
}
