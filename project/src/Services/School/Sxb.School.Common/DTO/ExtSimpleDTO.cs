using Sxb.School.Common.Enum;
using System;

namespace Sxb.School.Common.DTO
{
    public class ExtSimpleDTO
    {
        /// <summary>
        /// 学校ID
        /// </summary>
        public Guid SID { get; set; }
        /// <summary>
        /// 学部ID
        /// </summary>
        public Guid EID { get; set; }
        /// <summary>
        /// 学校名字
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 学部名字
        /// </summary>
        public string ExtName { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public string SchFType { get; set; }
        /// <summary>
        /// 省份代码
        /// </summary>
        public int ProvinceCode { get; set; }
        /// <summary>
        /// 市区代码
        /// </summary>
        public int CityCode { get; set; }
        /// <summary>
        /// 区域代码
        /// </summary>
        public int AreaCode { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public float? Latitude { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public float? Longitude { get; set; }
        /// <summary>
        /// 学校编号
        /// </summary>
        public int No { get; set; }
        /// <summary>
        /// 学校编号(Base32)
        /// </summary>
        public string ShortNo
        {
            get
            {
                return Framework.Foundation.UrlShortIdUtil.Long2Base32(No);
            }
        }
        /// <summary>
        /// 学段
        /// </summary>
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
        public bool? Lodging { get; set; }
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }
        /// <summary>
        /// 学费(元/年)
        /// </summary>
        public float? Tuition { get; set; }
        /// <summary>
        /// 学部评分
        /// </summary>
        public float? Score { get; set; }
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
        /// 学校认证
        /// </summary>
        public string Authentication { get; set; }
    }
}
