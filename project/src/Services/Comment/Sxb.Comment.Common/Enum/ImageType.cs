using System.ComponentModel;

namespace Sxb.Comment.Common.Enum
{
    public enum ImageType
    {
        [Description("未知")]
        Unknow = 0,
        [Description("点评")]
        Comment = 1,

        [Description("问答")]
        Answer = 2,

        [Description("问题")]
        Question = 3,

        [Description("举报")]
        Report = 4
    }
}
