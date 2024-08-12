using Kogel.Dapper.Extension.Attributes;
using Sxb.Comment.Common.Enum;
using System;

namespace Sxb.Comment.Common.Entity
{
    [Display(Rename = "SchoolCommentLikes")]
    public class GiveLikeInfo
    {
        [Identity]
        public int ID { get; set; }

        public LikeType LikeType { get; set; }
        /// <summary>
        /// 点评 / 问题 / 回复 ID
        /// </summary>
        public Guid SourceID { get; set; }
        public Guid ReplyID { get; set; }
        public Guid UserID { get; set; }
        /// <summary>
        /// 渠道标识
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 在添加实体时仓储自动分析
        /// </summary>
        public LikeStatus LikeStatus { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
