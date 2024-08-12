using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.PaidQA.Common.Entity
{
    /// <summary>
    /// 学段
    /// </summary>
    [Serializable]
    [Display(Rename = "Grade")]
    public partial class GradeInfo
    {
        /// <summary> 
        /// </summary> 
        [Identity]
        public Guid ID { get; set; }

        /// <summary> 
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 
        /// </summary> 
        public int Sort { get; set; }

        /// <summary> 
        /// </summary> 
        public bool IsValid { get; set; }
    }
}
