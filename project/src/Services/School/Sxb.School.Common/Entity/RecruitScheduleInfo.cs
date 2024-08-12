using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;
using System;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 招生日程
    /// </summary>
    [Serializable]
    //[Table(nameof(RecruitScheduleInfo))]
    public class RecruitScheduleInfo
    {
        [Identity(false)]
        [JsonIgnore]
        public Guid ID { get; set; }
        /// <summary>
        /// 招生信息ID
        /// </summary>
        //[JsonIgnore]
        //public Guid RecruitID { get; set; }
        /// <summary>
        /// 招生信息类型
        /// </summary>
        [JsonIgnore]
        public int? RecruitType { get; set; }
        /// <summary>
        /// 城市代码
        /// </summary>
        [JsonIgnore]
        public int? CityCode { get; set; }
        /// <summary>
        /// 区域代码
        /// </summary>
        [JsonIgnore]
        public int? AreaCode { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string StrDate { get; set; }
        /// <summary>
        /// 重要事项内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? Index { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        [JsonIgnore]
        public string SchFType { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
    }
}