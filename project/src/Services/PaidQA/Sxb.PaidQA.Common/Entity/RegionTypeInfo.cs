using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.PaidQA.Common.Entity
{
    [Serializable]
    [Display(Rename = "RegionType")]
    public class RegionTypeInfo
    {
        [Identity]
        public Guid ID { get; set; }
        /// <summary> 
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 
        /// </summary> 
        public Guid? PID { get; set; }

        /// <summary> 
        /// </summary> 
        public int? Sort { get; set; }

        /// <summary> 
        /// </summary> 
        public bool? IsValid { get; set; }
    }
}
