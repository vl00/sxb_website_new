using System.ComponentModel;

namespace Sxb.Comment.Common.Enum
{
    public enum LikeStatus
    {
        [Description("点赞")]
        Like = 1,

        [Description("取消点赞")]
        UnLike = -1
    }
}
