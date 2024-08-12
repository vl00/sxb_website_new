using System.ComponentModel;

namespace Sxb.School.Common.Enum
{
    /// <summary>
    /// 微信搜索政策大卡-招生政策-类型
    /// </summary>
    public enum WeChatRecruitType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknow = 0,
        /// <summary>
        /// 政策
        /// </summary>
        [Description("政策")]
        Policy = 1,
        /// <summary>
        /// 范围
        /// </summary>
        [Description("范围")]
        Scope = 2,
        /// <summary>
        /// 指引
        /// </summary>
        [Description("指引")]
        Guide = 3,
        /// <summary>
        /// 日程
        /// </summary>
        [Description("日程")]
        Schedule = 4,
        /// <summary>
        /// 咨询
        /// </summary>
        [Description("咨询")]
        Consult = 5
    }
}
