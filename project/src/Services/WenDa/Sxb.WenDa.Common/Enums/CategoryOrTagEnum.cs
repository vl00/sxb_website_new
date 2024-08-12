using System.ComponentModel;

namespace Sxb.WenDa.Common.Enums
{
    /// <summary>
    /// 分类还是标签
    /// </summary>
    public enum CategoryOrTagEnum
    {
        /// <summary>分类</summary>
        [Description("分类")]
        Category = 1,
        /// <summary>标签</summary>
        [Description("标签")]
        Tag = 2,
    }


}
