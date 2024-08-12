using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.PaidQA.Common.Entity
{
    [Serializable]
    [Display(Rename = "TalentSetting")]
    public class TalentSettingInfo
    {
        [Identity]
        public Guid TalentUserID { get; set; }
        /// <summary> 
        /// </summary> 
        public bool IsEnable { get; set; }
        /// <summary> 
        /// 咨询价格 
        /// </summary> 
        public decimal Price { get; set; }
        /// <summary>
        /// 达人认证等级ID
        /// </summary>
        public Guid TalentLevelTypeID { get; set; }
        public string JA_Covers { get; set; }
    }
}
