using System.ComponentModel;

namespace Sxb.Comment.Common.Enum
{
    public enum ReportStatus
    {
        [Description("未回复")]
        Unanswered = 0,
        [Description("已回复")]
        Answered = 1
    }
}
