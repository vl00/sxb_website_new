using System.ComponentModel;

namespace Sxb.ArticleMajor.Common.Enum
{
    /// <summary>
    /// 关键词->页面类型
    /// </summary>
    public enum KeywordPageType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknow = 0,
        /// <summary>
        /// 首页
        /// </summary>
        [Description("首页")]
        Top = 1,
        /// <summary>
        /// 二级页面
        /// </summary>
        [Description("二级页面")]
        LevelTwo = 2,
        /// <summary>
        /// 三级页面
        /// </summary>
        [Description("三级页面")]
        LevelThree = 3,
        /// <summary>
        /// 内容聚类
        /// </summary>
        [Description("内容聚类")]
        Tag = 4
    }
}
