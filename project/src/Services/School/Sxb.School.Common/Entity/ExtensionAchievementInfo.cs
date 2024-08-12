using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 学部升学成绩
    /// </summary>
    public class ExtensionAchievementInfo
    {
        [JsonIgnore]
        [Identity(false)]
        public Guid ID { get; set; }
        /// <summary>
        /// 学部ID
        /// </summary>
        [JsonIgnore]
        public Guid EID { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 字段Json
        /// </summary>
        [JsonIgnore]
        [Description("学部升学成绩-字段")]
        public string Items { get; set; }
        [Display(IsField = false)]
        public dynamic Items_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Items))
                {
                    try
                    {
                        return Items.FromJson<dynamic>();
                    }
                    catch
                    {
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// 表格Json
        /// </summary>
        [JsonIgnore]
        [Description("学部升学成绩-表格")]
        public string Tables { get; set; }
        [Display(IsField = false)]
        public IEnumerable<KeyValuePair<string, string>> Tables_Obj
        {
            get
            {
                return Tables.FromJsonSafe<IEnumerable<KeyValuePair<string, string>>>();
            }
        }
    }
}
