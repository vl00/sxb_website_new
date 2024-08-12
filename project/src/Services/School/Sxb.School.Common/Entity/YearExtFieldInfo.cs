using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.School.Common.Entity
{
    [Display(Rename = "YearExtField")]
    public class YearExtFieldInfo
    {
        public Guid EID { get; set; }
        public string Field { get; set; }
        public string Years { get; set; }
        [Display(IsField = false)]
        public IEnumerable<int> Years_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Years)) return Years.Split(',')?.Select(p =>
                {
                    if (int.TryParse(p, out int year)) return year;
                    else return default;
                }).Where(p => p != default);
                return default;
            }
        }
        public int Latest { get; set; }
    }
}
