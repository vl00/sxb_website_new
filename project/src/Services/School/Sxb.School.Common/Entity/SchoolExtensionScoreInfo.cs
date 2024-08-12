using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.School.Common.Entity
{
    [Display(Rename = "Score")]
    public class SchoolExtensionScoreInfo
    {
        [Identity(false)]
        public int ID { get; set; }
        /// <summary>
        /// 分数类型ID
        /// </summary>
        public int IndexID { get; set; }
        /// <summary>
        /// 分数值
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 学部ID
        /// </summary>
        public Guid EID { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Status { get; set; }
    }
}
