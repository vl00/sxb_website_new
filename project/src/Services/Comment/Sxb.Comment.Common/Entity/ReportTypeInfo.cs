using Kogel.Dapper.Extension.Attributes;

namespace Sxb.Comment.Common.Entity
{
    [Display(Rename = "ReportTypes")]
    public class ReportTypeInfo
    {
        [Identity]
        public int ID { get; set; }
        public string TypeName { get; set; }
        public int Sort { get; set; }
    }
}
