using Kogel.Dapper.Extension.Attributes;

namespace Sxb.School.Common.Entity
{
    [Display(Rename = "ScoreIndex")]
    public class ScoreIndexInfo
    {
        [Identity]
        public int ID { get; set; }
        public string Index_Name { get; set; }
        public int level { get; set; }
        public int ParentID { get; set; }
        public bool IsValid { get; set; }
    }
}
