using System.ComponentModel;

namespace Sxb.School.Common.Enum
{
    /// <summary>
    /// 微信搜索政策大卡-招生政策-子项-类型
    /// </summary>
    public enum WeChatRecruitItemType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknow = 0,
        /// <summary>
        /// 文章
        /// </summary>
        [Description("文章")]
        Article = 1,
        /// <summary>
        /// 附件
        /// </summary>
        [Description("附件")]
        Attachment = 2,
        /// <summary>
        /// 日程
        /// </summary>
        [Description("日程")]
        Schedule = 3
    }
}
