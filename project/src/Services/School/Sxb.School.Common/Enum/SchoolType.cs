using System.ComponentModel;

namespace Sxb.School.Common.Enum
{
    /// <summary>
    /// 学校类型 1公办  2民办  3国际 4外籍 80港澳台 99其它
    /// </summary>
    public enum SchoolType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknow = 0,
        /// <summary>
        /// 公办
        /// </summary>
        [Description("公办")]
        Public = 1,
        /// <summary>
        /// 民办
        /// </summary>
        [Description("民办")]
        Private = 2,
        /// <summary>
        /// 国际
        /// </summary>
        [Description("国际")]
        International = 3,
        /// <summary>
        /// 外籍
        /// </summary>
        [Description("外籍")]
        ForeignNationality = 4,
        /// <summary>
        /// 港澳台
        /// </summary>
        [Description("港澳台")]
        SAR = 80,
        /// <summary>
        /// 其它
        /// </summary>
        [Description("其它")]
        Other = 99
    }
}
