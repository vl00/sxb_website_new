using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.Comment.Common.Entity
{
    [Display(Rename = "SchoolTags")]
    public class CommentTagInfo
    {
        [Identity]
        public Guid ID { get; set; }
        public Guid TagID { get; set; }
        [Display(Rename = "SchoolID")]
        public Guid EID { get; set; }
        public Guid UserID { get; set; }
        [Display(Rename = "SchoolCommentID")]
        public Guid CommentID { get; set; }
    }
}
