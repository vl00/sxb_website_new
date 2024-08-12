using Kogel.Dapper.Extension.Attributes;
using Sxb.School.Common.Enum;
using System;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 在线学部信息
    /// </summary>
    [Display(Rename = "SchoolExtension")]
    public class SchoolExtensionInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 学校ID
        /// </summary>
        public Guid SID { get; set; }
        /// <summary>
        /// 学段
        /// </summary>
        public SchoolGradeType Grade { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public SchoolType Type { get; set; }
        /// <summary>
        /// 是否普惠
        /// </summary>
        public bool? Discount { get; set; }
        /// <summary>
        /// 是否双语
        /// </summary>
        public bool? Diglossia { get; set; }
        /// <summary>
        /// 是否中国人学校
        /// </summary>
        public bool? Chinese { get; set; }
        /// <summary>
        /// 学部名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 别称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 学部总类型
        /// </summary>
        public string SchFtype { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>
        public string Weixin { get; set; }
        /// <summary>
        /// 能否修改类型
        /// </summary>
        public bool? Allowedit { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public Guid? Creator { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyDateTime { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public Guid? Modifier { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool? IsValid { get; set; }
        /// <summary>
        /// 高德ID
        /// </summary>
        public Guid? ClaimedAmapEID { get; set; }
        /// <summary>
        /// 学部简介
        /// </summary>
        public string ExtIntro { get; set; }
        /// <summary>
        /// 学部编号
        /// </summary>
        public int No { get; set; }
        /// <summary>
        /// 是否有校车
        /// </summary>
        public bool HasSchoolBus { get; set; }
    }
}
