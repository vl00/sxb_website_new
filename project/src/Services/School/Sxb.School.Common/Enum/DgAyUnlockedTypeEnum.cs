using System.ComponentModel;

namespace Sxb.School.Common.Enum
{
    /// <summary>
    /// 解锁类型
    /// </summary>
    public enum DgAyUnlockedTypeEnum
    {
        /// <summary>
        /// 免费解锁
        /// </summary>
        [Description("免费解锁")]
        Free = 1,

        /// <summary>
        /// 解锁x元
        /// </summary>
        [Description("解锁x元")]
        X = 2,

        /// <summary>
        /// 解锁1元
        /// </summary>
        [Description("解锁1元")]
        One = 3,

        /// <summary>
        /// 无结果变解锁
        /// </summary>
        [Description("无结果")]
        NoResult = 0,
    }
}
