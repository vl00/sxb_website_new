using Newtonsoft.Json;
using Sxb.School.Common.Enum;
using System.Collections.Generic;

namespace Sxb.School.API.RequestContact.School
{
    public class GetSurroundExtsResponse
    {
        /// <summary>
        /// 学部列表
        /// </summary>
        public IEnumerable<dynamic> Exts { get; set; }
        /// <summary>
        /// 小区名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 小区位置
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 参考均价
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// 图片列表
        /// </summary>
        public IEnumerable<string> ImgUrls { get; set; }
        /// <summary>
        /// 开发商
        /// </summary>
        public string Developer { get; set; }
        /// <summary>
        /// 物业公司
        /// </summary>
        public string Property { get; set; }
        /// <summary>
        /// 建筑年代
        /// </summary>
        public int Year { get; set; }
    }

    public class ExtInfo
    {
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 学部名称
        /// </summary>
        public string ExtName { get; set; }
        /// <summary>
        /// 学段
        /// </summary>
        [JsonIgnore]
        public SchoolGradeType Grade { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 是否寄宿
        /// </summary>
        public LodgingType Lodging { get; set; }
        /// <summary>
        /// 学费(元/年)
        /// </summary>
        public dynamic Tuition { get; set; }
        /// <summary>
        /// 学部评分
        /// </summary>
        public float? Score { get; set; }
        /// <summary>
        /// 短ID
        /// </summary>
        public string ShortNo { get; set; }
        /// <summary>
        /// 教师数
        /// </summary>
        public int TeacherCount { get; set; }
        /// <summary>
        /// 学生数
        /// </summary>
        public int StudentCount { get; set; }
        /// <summary>
        /// 学制
        /// </summary>
        public EduSysType EduSysType { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
        [JsonIgnore]
        public string Authentication { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public double? Distance { get; set; }
    }
}
