using Kogel.Dapper.Extension.Attributes;
using System;
using System.ComponentModel;

namespace Sxb.PaidQA.Common.Entity
{
    /// <summary>
    /// 学部的达人
    /// </summary>
    public class ExtensionTalentInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        [Identity]
        public int ID { get; set; }
        /// <summary>
        /// 学部ID
        /// </summary>
        [Description("学部ID")]
        public Guid EID { get; set; }
        /// <summary>
        /// 达人用户ID
        /// </summary>
        [Description("达人用户ID")]
        public Guid TalentUserID { get; set; }
    }
}
