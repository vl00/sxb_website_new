using System;

namespace Sxb.Comment.API.RequestContract.Comment
{
    public class GetSchoolSelectedCommentRequest
    {
        public Guid EID { get; set; }
        public Guid? UserID { get; set; }
        public int Take { get; set; } = 1;
    }
}
