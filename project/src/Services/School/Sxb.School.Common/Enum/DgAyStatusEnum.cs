using System.ComponentModel;

namespace Sxb.School.Common.Enum
{
    /// <summary>
    /// 分析阶段
    /// </summary>
    public enum DgAyStatusEnum
    {
        /// <summary>
        /// 做题阶段
        /// </summary>
        [Description("做题阶段")]
        Todo = 1,

        /// <summary>
        /// 已分析出结果
        /// </summary>
        [Description("已分析出结果")]
        Analyzed = 2,

        /// <summary>
        /// 已解锁
        /// </summary>
        [Description("已解锁")]
        Unlocked = 3,

        
    }
}
