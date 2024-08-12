using System;

namespace Sxb.Comment.Common.DTO
{
    public class SchoolScoreCommentCountDto
    {
        public Guid Eid { get; set; }
        /// <summary>
        /// (家长评价)共n人点评
        /// </summary>
        public int CommentCount { get; set; }
    }
}
