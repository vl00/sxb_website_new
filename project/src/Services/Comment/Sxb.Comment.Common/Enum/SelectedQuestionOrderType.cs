using System.ComponentModel;

namespace Sxb.Comment.Common.Enum
{
    public enum SelectedQuestionOrderType
    {
        [Description("无排序")]
        None = -1,

        [Description("智能")]
        Intelligence = 0,

        [Description("发布时间")]
        CreateTime = 1,

        [Description("口碑评级")]
        QuestionTotal = 2,

        [Description("回复数")]
        Answer = 3
    }
}
