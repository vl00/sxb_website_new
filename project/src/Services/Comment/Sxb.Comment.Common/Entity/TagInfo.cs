using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.Comment.Common.Entity
{
    [Display(Rename = "Tags")]
    public class TagInfo
    {
        [Identity]
        public Guid ID { get; set; }
        public string Content { get; set; }
    }
}
