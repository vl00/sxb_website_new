using Sxb.School.Common.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sxb.School.API.RequestContact.WechatBigK
{
    public class GetRequest
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int? Year { get; set; }
        /// <summary>
        /// 区域代号
        /// </summary>
        [Required]
        public int AreaCode { get; set; }
        /// <summary>
        /// 学段
        /// </summary>
        [Required]
        public SchoolGradeType Grade { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [Required]
        public WeChatRecruitType Type { get; set; }
    }

    public class GetResponse
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 副标题
        /// </summary>
        public string SubTitle { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public dynamic Items { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public WeChatRecruitType Type { get; set; }
        public IEnumerable<int> Years { get; set; }
        /// <summary>
        /// 城市代码
        /// </summary>
        public int CityCode { get; set; }
        /// <summary>
        /// 区域代码
        /// </summary>
        public int AreaCode { get; set; }
    }
}
