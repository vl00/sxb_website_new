using Sxb.Framework.Foundation;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.School.Common.DTO
{
    public class SchoolExtensionDTO
    {
        /// <summary>
        /// 学校logo
        /// </summary>
        public string Logo { get; set; }
        public Guid Sid { get; set; }
        public Guid ExtId { get; set; }
        public long SchoolNo { get; set; }
        public string Intro { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public double? Distance { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 学校名字
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 学部别称Json
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 分部名字
        /// </summary>
        public string ExtName { get; set; }
        public string EName { get; set; }
        //enum SchoolGrade
        public byte Grade { get; set; }
        //enum SchoolType
        public byte Type { get; set; }
        public string TypeName
        {
            get
            {
                return Type switch
                {
                    1 => "公办",
                    2 => "民办",
                    3 => "国际",
                    _ => "未知",
                };
            }
        }
        public bool? Discount { get; set; }
        public bool? Diglossia { get; set; }
        public bool? Chinese { get; set; }
        /// <summary>
        /// 学生人数
        /// </summary>
        public int? Studentcount { get; set; }
        /// <summary>
        /// 教师人数
        /// </summary>
        public int? Teachercount { get; set; }
        /// <summary>
        /// 师生比例
        /// </summary>
        public float? TsPercent { get; set; }
        public int Province { get; set; }
        public int City { get; set; }
        public int Area { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }

        public double? Tuition { get; set; }
        public double? Applicationfee { get; set; }
        public string Otherfee { get; set; }
        public string Tel { get; set; }
        public string WebSite { get; set; }
        public string Address { get; set; }
        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }
        /// <summary>
        /// 有无饭堂
        /// </summary>
        public bool? Canteen { get; set; }
        /// <summary>
        ///伙食情况
        /// </summary>
        public string Meal { get; set; }
        /// <summary>
        /// 特色课程
        /// </summary>
        public string Characteristic { get; set; }
        /// <summary>
        /// 学校认证
        /// </summary>
        public string Authentication { get; set; }
        /// <summary>
        /// 外教比例
        /// </summary>
        public float? ForeignTea { get; set; }
        /// <summary>
        /// 出国方向
        /// </summary>
        public string Abroad { get; set; }
        /// <summary>
        /// 开放日
        /// </summary>
        public string OpenHours { get; set; }
        /// <summary>
        /// 行事日历
        /// </summary>
        public string Calendar { get; set; }
        /// <summary>
        /// 划片区域
        /// </summary>
        public string Range { get; set; }
        /// <summary>
        /// 课后管理
        /// </summary>
        public string AfterClass { get; set; }
        /// <summary>
        /// 对口学校
        /// </summary>
        public string CounterPart { get; set; }
        /// <summary>
        /// 派位学校
        /// </summary>
        public string Allocation { get; set; }

        /// <summary>
        /// 总评分
        /// </summary>
        public int? ExtPoint { get; set; }
        /// <summary>
        /// 点评数量
        /// </summary>
        public int? Comment { get; set; }

        /// <summary>
        /// taglist
        /// </summary>
        public IEnumerable<string> Tags { get; set; }


        /// <summary>
        /// 升学成绩的年份
        /// </summary>
        public int? AchYear { get; set; }
        /// <summary>
        /// 升学成绩
        /// </summary>
        public IEnumerable<KeyValueDTO<Guid, string, double, int>> Achievement { get; set; }

        public byte? Age { get; set; }
        public byte? MaxAge { get; set; }
        /// <summary>
        /// 招生对象
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// 招生比例
        /// </summary>
        public float? Proportion { get; set; }
        /// <summary>
        /// 录取分数线
        /// </summary>
        public string Point { get; set; }
        /// <summary>
        /// 招生人数
        /// </summary>
        public int? Count { get; set; }
        /// <summary>
        /// 课程设置
        /// </summary>
        public string Courses { get; set; }
        public string CourseAuthentication { get; set; }
        /// <summary>
        /// 课程特色
        /// </summary>
        public string CourseCharacteristic { get; set; }

        /// <summary>
        /// 线上体验课程
        /// </summary>
        public IEnumerable<KeyValueDTO<DateTime, string, byte>> ExperienceVideos { get; set; }
        /// <summary>
        /// 学校专访课程
        /// </summary>
        public IEnumerable<KeyValueDTO<DateTime, string, byte>> InterviewVideos { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Code { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingType LodgingType => LodgingUtil.Reason(Lodging, Sdextern);
        //学部信息 
        //public List<>

        /// <summary>
        /// 是否有校车
        /// </summary>
        public bool HasSchoolBus { get; set; }
        /// <summary>
        /// 学校学制
        /// </summary>
        public EduSysType? EduSysType { get; set; }

        public string EduSysTypeName
        {
            get
            {

                return EduSysType?.GetDescription();
            }
        }

        public string GradeName
        {
            get
            {
                return Grade switch
                {
                    1 => "幼儿园",
                    2 => "小学",
                    3 => "初中",
                    4 => "高中",
                    _ => ""
                };
            }
        }

        public SchFType0 SchFType { get; set; }

        /// <summary>
        /// 学校图片列表
        /// </summary>
        public IEnumerable<SchoolImageDTO> SchoolImages { get; set; }

        public IEnumerable<SchoolImageDTO> PrincipalImages => SchoolImages.Where(s => s.Type == SchoolImageType.Principal);
        public IEnumerable<SchoolImageDTO> TeacherImages => SchoolImages.Where(s => s.Type == SchoolImageType.Teacher);
        public IEnumerable<SchoolImageDTO> HardwareImages => SchoolImages.Where(s => s.Type == SchoolImageType.Hardware);
        public IEnumerable<SchoolImageDTO> StudentImages => SchoolImages.Where(s => s.Type == SchoolImageType.CommunityActivities);

        /// <summary>
        /// 学校首张图片
        /// </summary>
        public string SchoolImageUrl { get; set; }

        /// <summary>
        /// 考试科目
        /// </summary>
        public string Subjects { get; set; }

        /// <summary>
        /// 学费/年
        /// </summary>
        public string TuitionPerYearFee { get; set; }

        /// <summary>
        /// 考试科目 key列表
        /// </summary>
        public List<KeyValuePair<string, string>> SubjectsList => CommonHelper.TryJsonDeserializeObject(Subjects, new List<KeyValuePair<string, string>>());

        /// <summary>
        /// 课程设置 key列表  ["幼儿启蒙教育", "幼儿启蒙教育"]
        /// [{"Key":"幼儿启蒙教育","Value":"14acb08d-575e-4171-ac00-8db70bbe59b4"}]
        /// </summary>
        public List<KeyValuePair<string, string>> CoursesList => CommonHelper.TryJsonDeserializeObject(Courses, new List<KeyValuePair<string, string>>());

        /// <summary>
        /// 学校认证 key列表  ["省级示范性", "省级示范性"]
        /// [{"Key":"省级示范性","Value":"7140698c-18b7-4320-9ed8-529c0e18db0f"}]
        /// </summary>
        public List<KeyValuePair<string, string>> AuthenticationList => CommonHelper.TryJsonDeserializeObject(Authentication, new List<KeyValuePair<string, string>>());

        /// <summary>
        /// 招生对象 key列表  ["本市学籍", "本市居住证"]
        /// [{"Key":"本市学籍","Value":"fe60cbd6-b3cb-4d43-a668-ba9602bee321"},{"Key":"本市户籍","Value":"6e6b2cde-802b-4e2e-be03-a2333269fe84"},{ "Key":"本市居住证","Value":"bdfba0f8-0abe-4fcd-93e7-5a9f1acee6fe"}]
        /// </summary>
        public List<KeyValuePair<string, string>> TargetList => CommonHelper.TryJsonDeserializeObject(Target, new List<KeyValuePair<string, string>>());

        /// <summary>
        /// 出国方向 key列表
        /// </summary>
        public List<KeyValuePair<string, string>> AbroadList => CommonHelper.TryJsonDeserializeObject(Abroad, new List<KeyValuePair<string, string>>());

        /// <summary>
        /// 招生方式[Json 字符串数组结构]
        /// </summary>
        public string RecruitWay { get; set; }


        /// <summary>
        /// 对RecruitWay中的Json串解析为列表
        /// </summary>
        public List<string> RecruitWays
        {
            get
            {
                List<string> defatultRecruitWays = new List<string>();
                if (string.IsNullOrEmpty(RecruitWay))
                {
                    return defatultRecruitWays;
                }
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(this.RecruitWay);
                }
                catch
                {
                    return defatultRecruitWays;
                }

            }
        }

        /// <summary>
        /// 学校公众号名称
        /// </summary>
        public string OAName { get; set; }

        /// <summary>
        /// 学校公众号AppID
        /// </summary>
        public string OAAppID { get; set; }

        /// <summary>
        /// 学校公众号账号
        /// </summary>
        public string OAAccount { get; set; }
        /// <summary>
        /// 学校小程序名称
        /// </summary>
        public string MPName { get; set; }

        /// <summary>
        /// 学校小程序AppID
        /// </summary>
        public string MPAppID { get; set; }

        /// <summary>
        /// 学校小程序账号
        /// </summary>
        public string MPAccount { get; set; }
        /// <summary>
        /// 学校视频号名称
        /// </summary>
        public string VAName { get; set; }

        /// <summary>
        /// 学校视频号AppID
        /// </summary>
        public string VAAppID { get; set; }

        /// <summary>
        /// 学校视频号账号
        /// </summary>
        public string VAAccount { get; set; }
    }
}
