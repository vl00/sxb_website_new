using System;

namespace Sxb.Comment.Common.DTO
{
    public class CommentReplyDTO
    {
        public Guid ReplyID { get; set; }

        public string ReplyContent { get; set; }

        public DateTime ReplyTime { get; set; }

        public Guid ReplayUserID { get; set; }

        public Guid? ParentID { get; set; }

        public Guid? ParentUserID { get; set; }
    }
}
