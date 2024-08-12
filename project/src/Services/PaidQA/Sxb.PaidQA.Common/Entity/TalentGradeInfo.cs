using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.PaidQA.Common.Entity
{
    [Display(Rename = "TalentGrade")]
    public class TalentGradeInfo
    {
        /// <summary> 
        /// </summary> 
        [Identity]
        public Guid ID { get; set; }
        public Guid GradeID { get; set; }
        public Guid TalentUserID { get; set; }
    }
}
