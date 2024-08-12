using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.PaidQA.Common.Entity
{
    [Display(Rename = "LevelType")]
    public class LevelTypeInfo
    {
        /// <summary> 
        /// </summary> 
        [Identity]
        public Guid ID { get; set; }
        /// <summary> 
        /// </summary> 
        public string Name { get; set; }
        public int Sort { get; set; }
    }
}
