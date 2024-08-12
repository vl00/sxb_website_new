using System.ComponentModel;

namespace Sxb.Comment.Common.Enum
{
    public enum LikeType
    {
        [Description("点评")]
        Comment = 1,

        [Description("问题")]
        Quesiton = 2,

        [Description("回复")]
        Reply = 3,

        [Description("答题")]
        Answer = 4
    }
}
