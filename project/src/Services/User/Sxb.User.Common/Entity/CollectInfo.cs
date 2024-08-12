using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.User.Common.Entity
{
    [Display(Rename = "Collection")]
    public class CollectionInfo
    {
        public Guid DataID { get; set; }
        public int DataType { get; set; }
        public Guid UserID { get; set; }
        public DateTime Time { get; set; }
    }
}
