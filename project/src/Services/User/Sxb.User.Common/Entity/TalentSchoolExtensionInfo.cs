using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.User.Common.Entity
{
    [Display(Rename = "talentSchoolExtension")]
    public class TalentSchoolExtensionInfo
    {
        [Identity]
        public Guid ID { get; set; }
        public Guid TalentID { get; set; }
        public Guid EID { get; set; }
    }
}
