using System.ComponentModel;

namespace Sxb.School.Common.Enum
{
    /// <summary>
    /// 微信搜索政策大卡-招生政策-文章-副标题对齐方式
    /// </summary>
    public enum SubTitleTextAlignType
    {
        Unknow = 0,
        /// <summary>
        /// 左
        /// </summary>
        [Description("左")]
        Left = 1,
        /// <summary>
        /// 中
        /// </summary>
        [Description("中")]
        Center = 2,
        /// <summary>
        /// 右
        /// </summary>
        [Description("右")]
        Right = 3
    }
}
