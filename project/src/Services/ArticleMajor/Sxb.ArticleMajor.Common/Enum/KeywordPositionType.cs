using System.ComponentModel;

namespace Sxb.ArticleMajor.Common.Enum
{
    /// <summary>
    /// 关键词->位置类型
    /// </summary>
    public enum KeywordPositionType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknow = 0,
        /// <summary>
        /// 导航栏
        /// </summary>
        [Description("导航栏")]
        Position1 = 1,
        /// <summary>
        /// 时事热点
        /// </summary>
        [Description("时事热点")]
        Position2 = 2,
        /// <summary>
        /// 右侧栏
        /// </summary>
        [Description("右侧栏")]
        Position3 = 3
    }
}
