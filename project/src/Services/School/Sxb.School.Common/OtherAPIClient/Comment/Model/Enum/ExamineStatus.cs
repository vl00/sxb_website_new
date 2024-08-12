using System.ComponentModel;

namespace Sxb.School.Common.OtherAPIClient.Comment.Model.Enum
{
    public enum ExamineStatus
    {
        /// <summary>
        /// 无状态
        /// </summary>
        [Description("未知")]
        Unknown = 0,

        /// <summary>
        /// 未阅
        /// </summary>
        [Description("未阅")]
        Unread = 1,

        /// <summary>
        /// 已阅
        /// </summary>
        [Description("已阅")]
        Readed = 2,

        /// <summary>
        /// 精选
        /// </summary>
        [Description("已加精")]
        Highlight = 3,

        /// <summary>
        /// 已屏蔽
        /// </summary>
        [Description("已屏蔽")]
        Block = 4
    }
}
