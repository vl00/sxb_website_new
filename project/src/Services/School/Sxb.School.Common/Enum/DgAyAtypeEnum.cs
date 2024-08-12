using System.ComponentModel;

namespace Sxb.School.Common.Enum
{
    /// <summary>
    /// 分析类型
    /// </summary>
    public enum DgAyAtypeEnum
    {
        /// <summary>
        /// 对口
        /// </summary>
        [Description("对口入学")]
        Cp = 1,

        /// <summary>
        /// 统筹
        /// </summary>
        [Description("统筹入学")]
        Ov = 2,

        /// <summary>
        /// 积分入学
        /// </summary>
        [Description("积分入学")]
        Jf = 3,

        /// <summary>
        /// 查找心仪民办小学
        /// </summary>
        [Description("查找心仪民办小学")]
        Mb = 4,
    }

    /// <summary>
    /// 对口入学-小升初方式
    /// </summary>
    public enum DgAyCpPriToMidTypeEnum
    {
        /// <summary>
        /// 对口直升
        /// </summary>
        [Description("对口直升")]
        Heli = 1,
        /// <summary>
        /// 电脑派位
        /// </summary>
        [Description("电脑派位")]
        PcAssign = 2,
    }
}
