using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Sxb.Framework.Foundation;
using System.Linq;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 开设的项目与课程
    /// </summary>
    [Serializable]
    public class OnlineSchoolProjectInfo
    {
        [JsonIgnore]
        [Identity(false)]
        public Guid ID { get; set; }
        [JsonIgnore]
        public Guid EID { get; set; }
        [Description("名称")]
        public string Name { get; set; }
        [JsonIgnore]
        [Description("详情")]
        public string ItemJson { get; set; }
        [Display(IsField = false)]
        public IEnumerable<string> Item_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ItemJson))
                {
                    var result = ItemJson.FromJsonSafe<string[]>();
                    if (result?.Any(p => !string.IsNullOrWhiteSpace(p)) == true) return result;
                }
                return default;
            }
        }
        public bool IsDeleted { get; set; }
    }
}