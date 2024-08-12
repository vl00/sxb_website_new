using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Sxb.ArticleMajor.Common.Enum
{
    /// <summary>
    /// 关键词->站点类型
    /// </summary>
    public enum KeywordSiteType
    {
        /// <summary>
        /// 主站
        /// </summary>
        [Description("主站")]
        Unknow = 0,
        /// <summary>
        ///幼儿教育
        /// </summary>
        [Description("幼儿教育")]
        YEJY = 1,
        /// <summary>
        /// 小学教育
        /// </summary>
        [Description("小学教育")]
        XXJY = 2,
        /// <summary>
        /// 中学教育
        /// </summary>
        [Description("中学教育")]
        ZXJY = 3,
        /// <summary>
        /// 中职教育
        /// </summary>
        [Description("中职教育")]
        ZZJY = 4,
        /// <summary>
        /// 高中教育
        /// </summary>
        [Description("高中教育")]
        GZJY = 5,
        /// <summary>
        /// 素质教育
        /// </summary>
        [Description("素质教育")]
        SZJY = 6,
        /// <summary>
        /// 国际教育
        /// </summary>
        [Description("国际教育")]
        GJJY = 7
    }
}
