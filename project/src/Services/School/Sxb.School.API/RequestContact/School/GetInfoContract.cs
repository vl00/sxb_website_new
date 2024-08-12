using Newtonsoft.Json;
using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.OtherAPIClient.PaidQA.Model.EntityExtend;
using System;
using System.Collections.Generic;

namespace Sxb.School.API.RequestContact.School
{
    public class GetInfoRequest
    {
        public Guid EID { get; set; }
        public string SchoolNo { get; set; }
    }
    public class GetInfoResponse
    {
        /// <summary>
        /// 学校类型
        /// </summary>
        public string SchFType { get; set; }
        /// <summary>
        /// 学部ID
        /// </summary>
        public Guid EID { get; set; }
        /// <summary>
        /// 开设的项目与课程
        /// </summary>
        public IEnumerable<OnlineSchoolProjectInfo> Projects { get; set; }
        /// <summary>
        /// 课程
        /// </summary>
        public IEnumerable<string> Courses { get; set; }
        /// <summary>
        /// 课程特色
        /// </summary>
        public string CourseCharacteristic { get; set; }
        /// <summary>
        /// 课程认证
        /// </summary>
        public IEnumerable<string> CourseAuths { get; set; }
        public string SchoolName { get; set; }
        public string SchoolExtName { get; set; }
        /// <summary>
        /// 学部别称
        /// </summary>
        public IEnumerable<string> ExtNicknames { get; set; }
        /// <summary>
        /// 学校认证
        /// </summary>
        public object SchoolAuths { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int Grade { get; set; }
        /// <summary>
        /// 学校概况
        /// </summary>
        public SchoolOverviewEx Overview { get; set; }
        /// <summary>
        /// 招生信息
        /// </summary>
        public IEnumerable<Recruit> Recruits { get; set; }
        public IEnumerable<KeyValuePair<int, IEnumerable<int>>> RecruitYears { get; set; }
        /// <summary>
        /// 对口学校
        /// </summary>
        public IEnumerable<(string ExtName, string Address, string EID, string SchoolNo)> CounterParts { get; set; }
        public IEnumerable<string> CounterPartYears { get; set; }
        /// <summary>
        /// 派位学校
        /// </summary>
        public IEnumerable<(string ExtName, string Address, string EID, string SchoolNo)> Allocations { get; set; }
        public IEnumerable<string> AllocationYears { get; set; }
        /// <summary>
        /// 指标分配
        /// </summary>
        public IEnumerable<OnlineSchoolQuotaInfo> Quotas { get; set; }
        public IEnumerable<KeyValuePair<int, IEnumerable<int>>> QuotaYears { get; set; }
        /// <summary>
        /// 分数线
        /// </summary>
        public IEnumerable<ExtensionFractionInfo> Fractions { get; set; }
        //public IEnumerable<SchoolFractionInfo2> Fractions2 { get; set; }
        //public object Fractions_Obj { get; set; }
        public IEnumerable<KeyValuePair<int, IEnumerable<int>>> FractionYears { get; set; }
        //public IEnumerable<KeyValuePair<int, IEnumerable<int>>> Fraction2Years { get; set; }
        /// <summary>
        /// 升学成绩
        /// </summary>
        public IEnumerable<ExtensionAchievementInfo> Achievements { get; set; }
        public IEnumerable<int> AchievemenYears { get; set; }
        /// <summary>
        /// 学校图册
        /// </summary>
        public dynamic Images { get; set; }
        /// <summary>
        /// 学校视频
        /// </summary>
        public IEnumerable<dynamic> Videos { get; set; }
        /// <summary>
        /// 推荐达人
        /// </summary>
        public TalentDetailExtend Talent { get; set; }
        /// <summary>
        /// 其他分部
        /// </summary>
        public IEnumerable<KeyValueDTO<Guid>> OtherExt { get; set; }
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
        public IEnumerable<int> CostYears { get; set; }
        /// <summary>
        /// 本区招生政策
        /// </summary>
        [JsonIgnore]
        public string AreaRecruitPlan { get; set; }
        public object AreaRecruitPlan_Obj
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<dynamic>(AreaRecruitPlan);
                }
                catch { }
                return null;
            }
        }
        /// <summary>
        /// 学校动态
        /// </summary>
        public IEnumerable<dynamic> ExtArticles { get; set; }
        /// <summary>
        /// 推荐课程/机构
        /// </summary>
        public OrgLessonDTO OrgLessons { get; set; }
        /// <summary>
        /// 区域招生政策
        /// </summary>
        public IEnumerable<AreaRecruitPlanInfo> AreaRecruitPlans { get; set; }
        public IEnumerable<int> AreaRecruitPlanYears { get; set; }
        /// <summary>
        /// 推荐学部
        /// </summary>
        public dynamic RecommendExtensions { get; set; }
        public dynamic CommentTags { get; set; }
        public Guid SchoolID { get; set; }
        public dynamic SeletedComments { get; set; }
        public dynamic SeletedQuestions { get; set; }
        public dynamic QuestionTags { get; set; }
        /// <summary>
        /// 学部短No
        /// </summary>
        public string ShortNo { get; set; }
        /// <summary>
        /// 学校标签
        /// </summary>
        public IList<string> Tags { get; set; }
        /// <summary>
        /// 学校类型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 学校英文名
        /// </summary>
        public string EnglishName { get; set; }
        public dynamic ScoreTree { get; set; }
    }
    public class SchoolOverviewEx : SchoolOverview
    {

    }
}
