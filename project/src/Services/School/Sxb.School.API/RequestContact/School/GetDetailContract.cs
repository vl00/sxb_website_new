using Newtonsoft.Json;
using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Common.OtherAPIClient.PaidQA.Model.EntityExtend;
using System;
using System.Collections.Generic;

namespace Sxb.School.API.RequestContact.School
{
    public class GetDetailResponse
    {
        public bool IsCollected { get; set; }
        public Guid EID { get; set; }
        /// <summary>
        /// 学部短No
        /// </summary>
        public string ShortNo { get; set; }
        /// <summary>
        /// 学校类型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 学段
        /// </summary>
        public int Grade { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 学部名称
        /// </summary>
        public string ExtName { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public string SchFType { get; set; }
        /// <summary>
        /// 学部别称
        /// </summary>
        public IEnumerable<string> ExtNicknames { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// 学校主图Url
        /// </summary>
        public string SchoolImageUrl { get; set; }
        /// <summary>
        /// 学校图片
        /// </summary>
        public List<SchoolImageDTO> SchoolImages { get; set; }
        /// <summary>
        /// 学校英文名
        /// </summary>
        public string EnglishName { get; set; }
        /// <summary>
        /// 学校标签
        /// </summary>
        public IList<string> Tags { get; set; }
        /// <summary>
        /// 学校概况
        /// </summary>
        public SchoolOverview Overview { get; set; }
        /// <summary>
        /// 招生信息
        /// </summary>
        public IEnumerable<Recruit> Recruits { get; set; }
        /// <summary>
        /// 对口学校
        /// </summary>
        public IEnumerable<(string ExtName, string Address, string EID, string SchoolNo)> CounterPart { get; set; }
        /// <summary>
        /// 派位学校
        /// </summary>
        public IEnumerable<(string ExtName, string Address, string EID, string SchoolNo)> Allocation { get; set; }
        /// <summary>
        /// 指标分配
        /// </summary>
        public IEnumerable<OnlineSchoolQuotaInfo> Quotas { get; set; }
        /// <summary>
        /// 升学成绩
        /// </summary>
        public IEnumerable<Achievement> Achievements { get; set; }
        /// <summary>
        /// 分数线
        /// </summary>
        public IEnumerable<ExtensionFractionInfo> Fractions { get; set; }
        //public IEnumerable<SchoolFractionInfo2> Fractions2 { get; set; }
        //public object Fractions_Obj { get; set; }
        /// <summary>
        /// 学校视频
        /// </summary>
        public IEnumerable<dynamic> SchoolVideo { get; set; }
        /// <summary>
        /// 推荐达人
        /// </summary>
        public TalentDetailExtend Talent { get; set; }
        /// <summary>
        /// 家长问答
        /// </summary>
        public QuestionDTO Question { get; set; }
        /// <summary>
        /// 学校动态
        /// </summary>
        public IEnumerable<dynamic> ExtArticles { get; set; }
        /// <summary>
        /// 其他学部
        /// </summary>
        public IEnumerable<KeyValueDTO<Guid>> ExtSchools { get; set; }
        /// <summary>
        /// 推荐课程/机构
        /// </summary>
        public OrgLessonDTO OrgLessons { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        public bool IsAuth { get; set; }

        /// <summary>
        /// 申请费用
        /// </summary>
        public string ApplyCost { get; set; }
        /// <summary>
        /// 学费
        /// </summary>
        public string Tuition { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        [JsonIgnore]
        public string OtherCost { get; set; }
        public object OtherCost_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(OtherCost);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 本区招生政策
        /// </summary>
        public IEnumerable<AreaRecruitPlanInfo> AreaRecruitPlan { get; set; }
        /// <summary>
        /// 分数相关
        /// </summary>
        public SchoolScore Scores { get; set; }
        /// <summary>
        /// 排行相关
        /// </summary>
        public dynamic Rankings { get; set; }
        public dynamic ScoreTree { get; set; }
        public dynamic SeletedComment { get; set; }
        /// <summary>
        /// 推荐学部
        /// </summary>
        public dynamic RecommendExtensions { get; set; }
    }
    public class SchoolOverview
    {
        /// <summary>
        /// 学生数量
        /// </summary>
        public int StudentQuantity { get; set; }
        /// <summary>
        /// 师生比
        /// </summary>
        public string TeacherStudentScale { get; set; }
        /// <summary>
        /// 是否有校车
        /// </summary>
        public bool? HasSchoolBus { get; set; }
        /// <summary>
        /// 是否有饭堂
        /// </summary>
        public bool? HasCanteen { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingType LodgingType { get; set; }
        /// <summary>
        /// 出国方向
        /// </summary>
        [JsonIgnore]
        public string Abroad { get; set; }
        public object Abroad_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(Abroad);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 学校地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 官网
        /// </summary>
        public string WebSite { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        public string Intro { get; set; }
        /// <summary>
        /// 招生方式
        /// </summary>
        public object RecruitWay { get; set; }
        /// <summary>
        /// 城市代号
        /// </summary>
        public int CityCode { get; set; }
    }
    public class Recruit : OnlineSchoolRecruitInfo
    {
        /// <summary>
        /// 招生日程
        /// </summary>
        public IEnumerable<RecruitScheduleInfo> RecruitSchedules { get; set; }
    }

    public class Achievement : ExtensionAchievementInfo
    {
        public IEnumerable<KeyValuePair<string, string>> Forwards { get; set; }
    }

    public class SchoolScore
    {
        /// <summary>
        /// 总分排名百分比
        /// </summary>
        public int? ScoreRanking { get; set; }
        /// <summary>
        /// 总分
        /// </summary>
        public float? TotalScore { get; set; }
        /// <summary>
        /// 教学
        /// </summary>
        public float? Teaching { get; set; }
        /// <summary>
        /// 生源
        /// </summary>
        public float? StudentSource { get; set; }
        /// <summary>
        /// 周边
        /// </summary>
        public float? Surround { get; set; }
        /// <summary>
        /// 口碑
        /// </summary>
        public float? Prestige { get; set; }
    }
}