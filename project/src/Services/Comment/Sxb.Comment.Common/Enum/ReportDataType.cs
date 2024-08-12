using System.ComponentModel;

namespace Sxb.Comment.Common.Enum
{
    public enum ReportDataType
    {
        [Description("未知")]
        Unknow = 0,
        [Description("数据源")]
        DataSource = 1,
        [Description("回复")]
        Replay = 2
    }
}
