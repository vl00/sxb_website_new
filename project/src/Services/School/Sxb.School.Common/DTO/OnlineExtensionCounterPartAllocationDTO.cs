using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;

namespace Sxb.School.Common.DTO
{
    [Display(Rename = "OnlineSchoolExtContent")]
    public class OnlineExtensionCounterPartAllocationDTO
    {
        public Guid EID { get; set; }

        [JsonIgnore]
        public string CounterPart { get; set; }

        [Display(IsField = false)]
        public IEnumerable<KeyValuePair<string, Guid>> CounterPart_Obj
        {
            get { return CounterPart.FromJsonSafe<IEnumerable<KeyValuePair<string, Guid>>>(); }
        }

        [JsonIgnore]
        public string Allocation { get; set; }

        [Display(IsField = false)]
        public IEnumerable<KeyValuePair<string, Guid>> Allocation_Obj
        {
            get { return Allocation.FromJsonSafe<IEnumerable<KeyValuePair<string, Guid>>>(); }
        }
    }
}
