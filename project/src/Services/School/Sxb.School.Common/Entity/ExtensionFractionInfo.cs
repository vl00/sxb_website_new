using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;
using Sxb.Framework.Foundation;
using Sxb.School.Common.Enum;
using System;
using System.ComponentModel;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 学部分数线
    /// </summary>
    public class ExtensionFractionInfo
    {
        [JsonIgnore]
        [Identity(false)]
        public Guid ID { get; set; }
        [JsonIgnore]
        public Guid EID { get; set; }
        /// <summary>
        /// 类型
        /// <para>1.中考分数线</para>
        /// <para>2.中考录取分数线</para>
        /// <para>3.分数线</para>
        /// </summary>
        [Description("学部分数线类型")]
        public ExtensionFractionType Type { get; set; }
        [Display(IsField = false)]
        public string TypeName
        {
            get
            {
                return Type.GetDescription();
            }
        }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 字段
        /// </summary>
        [JsonIgnore]
        [Description("学部分数线字段")]
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
        /// 表格
        /// </summary>
        [JsonIgnore]
        [Description("学部分数线表格")]
        public string Tables { get; set; }
        [Display(IsField = false)]
        public dynamic Tables_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Tables))
                {
                    try
                    {
                        return Tables.FromJson<dynamic>();
                    }
                    catch
                    {
                    }
                }
                return null;
            }
        }
    }
}