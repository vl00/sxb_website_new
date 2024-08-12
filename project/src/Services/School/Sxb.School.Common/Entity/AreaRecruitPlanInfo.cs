using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 区域招生政策
    /// </summary>
    [Serializable]
    //[Table(nameof(AreaRecruitPlanInfo))]
    public class AreaRecruitPlanInfo
    {
        [Identity(false)]
        [JsonIgnore]
        public Guid ID { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        [JsonIgnore]
        public string SchFType { get; set; }
        /// <summary>
        /// 区域代号
        /// </summary>
        [JsonIgnore]
        public string AreaCode { get; set; }
        /// <summary>
        /// 链接数据
        /// </summary>
        [JsonIgnore]
        public string UrlData { get; set; }
        [Display(IsField = false)]
        public IEnumerable<string[]> UrlData_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(UrlData)) return UrlData.FromJsonSafe<IEnumerable<string[]>>();
                return default;
            }
        }
        /// <summary>
        /// 积分办法
        /// </summary>
        public string PointMethod { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
    }
}