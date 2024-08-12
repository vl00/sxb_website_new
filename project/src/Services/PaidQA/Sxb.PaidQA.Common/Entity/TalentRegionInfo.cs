using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.PaidQA.Common.Entity
{
    [Serializable]
    [Display(Rename = "TalentRegion")]
    public class TalentRegionInfo
    {
        /// <summary> 
        /// </summary> 
        [Identity]
        public Guid ID { get; set; }

        /// <summary> 
        /// </summary> 
        public Guid? UserID { get; set; }

        /// <summary> 
        /// </summary> 
        public Guid? RegionTypeID { get; set; }
    }
}
