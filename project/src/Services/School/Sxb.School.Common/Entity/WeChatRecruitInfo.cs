using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;
using Sxb.School.Common.Enum;
using System;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 微信搜索政策大卡-招生政策
    /// </summary>
    public class WeChatRecruitInfo
    {
        /// <summary>
        /// 就是ID
        /// </summary>
        [JsonIgnore]
        [Identity(false)]
        public Guid ID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 城市代码
        /// </summary>
        public int CityCode { get; set; }
        /// <summary>
        /// 区域代码
        /// </summary>
        public int AreaCode { get; set; }
        /// <summary>
        /// 副标题
        /// </summary>
        public string SubTitle { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public WeChatRecruitType Type { get; set; }
        /// <summary>
        /// 学段
        /// </summary>
        public SchoolGradeType Grade { get; set; }
    }
}
