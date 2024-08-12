using System.ComponentModel;

namespace Sxb.School.Common.Enum
{
    public enum ExtensionFractionType
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        Unknow = 0,
        /// <summary>
        /// 分数线
        /// </summary>
        [Description("分数线")]
        FSX = 1,
        /// <summary>
        /// 中考分数线
        /// </summary>
        [Description("中考分数线")]
        ZKFSX = 2,
        /// <summary>
        /// 中考录取分数线
        /// </summary>
        [Description("中考录取分数线")]
        ZKLQFSX = 3,
        /// <summary>
        /// 分数线
        /// <para>联招版</para>
        /// </summary>
        [Description("分数线")]
        FSX2 = 4
    }
}
