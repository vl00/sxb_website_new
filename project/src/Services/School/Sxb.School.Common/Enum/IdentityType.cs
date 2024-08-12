using System.ComponentModel;

namespace Sxb.School.Common.Enum
{
    public enum IdentityType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknow = 0,
        /// <summary>
        /// 校方
        /// </summary>
        [Description("校方")]
        Offcial = 1,
        /// <summary>
        /// 家长
        /// </summary>
        [Description("家长")]
        Parent = 2,
        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 3

    }
}
