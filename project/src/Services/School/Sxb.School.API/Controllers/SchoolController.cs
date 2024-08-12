using Less.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.School.API.Application.Query;
using Sxb.School.API.Infrastructures.Services;
using Sxb.School.API.Models;
using Sxb.School.API.RequestContact.School;
using Sxb.School.Common;
using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Common.OtherAPIClient.Article;
using Sxb.School.Common.OtherAPIClient.Comment;
using Sxb.School.Common.OtherAPIClient.Comment.Models;
using Sxb.School.Common.OtherAPIClient.PaidQA;
using Sxb.School.Common.OtherAPIClient.PaidQA.Model.EntityExtend;
using Sxb.School.Common.OtherAPIClient.User;
using Sxb.School.Common.OtherAPIClient.WxWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public partial class SchoolController : ControllerBase
    {
        readonly ISchoolExtensionQuery _schoolExtensionQuery;
        readonly ISchoolRecruitQuery _schoolRecruitQuery;
        readonly ISchoolOverViewQuery _schoolOverViewQuery;
        readonly IAreaRecruitPlanQuery _areaRecruitPlanQuery;
        readonly ISchoolExtensionLevelQuery _schoolExtensionLevelQuery;
        readonly ISchoolAchievementQuery _schoolAchievementQuery;
        readonly ISchoolQuotaQuery _schoolQuotaQuery;
        readonly ISchoolFractionQuery _schoolFractionQuery;
        readonly ICommentAPIClient _commentAPIClient;
        readonly IArticleAPIClient _articleAPIClient;
        readonly IPaidQAAPIClient _paidQAAPIClient;
        readonly IEasyRedisClient _easyRedisClient;
        readonly ISchoolScoreQuery _schoolScoreQuery;
        readonly ISchoolProjectQuery _schoolProjectQuery;
        readonly IUserAPIClient _userAPIClient;
        readonly ICorrectionMsgQuery _correctionMsgQuery;
        readonly IVerifyCodeAPIClient _verifyCodeAPIClient;
        readonly IWxWorkAPIClient _wxWorkAPIClient;
        readonly IWeChatGatewayService _weChatGatewayService;
        readonly IConfiguration _configuration;
        readonly ISchoolGeneralCommentQuery _schoolGeneralCommentQuery;

        public SchoolController(ISchoolExtensionQuery schoolExtensionQuery, ISchoolRecruitQuery schoolRecruitQuery, ISchoolOverViewQuery schoolOverViewQuery,
            IAreaRecruitPlanQuery areaRecruitPlanQuery, ISchoolExtensionLevelQuery schoolExtensionLevelQuery, ISchoolAchievementQuery schoolAchievementQuery,
            ISchoolQuotaQuery schoolQuotaQuery, ISchoolFractionQuery schoolFractionQuery, ICommentAPIClient commentAPIClient, IArticleAPIClient articleAPIClient,
            IPaidQAAPIClient paidQAAPIClient, IEasyRedisClient easyRedisClient, ISchoolScoreQuery schoolScoreQuery, ISchoolProjectQuery schoolProjectQuery,
            IUserAPIClient userAPIClient, ICorrectionMsgQuery correctionMsgQuery, IVerifyCodeAPIClient verifyCodeAPIClient, IWxWorkAPIClient wxWorkAPIClient,
            ISchoolGeneralCommentQuery schoolGeneralCommentQuery,
            IWeChatGatewayService weChatGatewayService, IConfiguration configuration)
        {
            _wxWorkAPIClient = wxWorkAPIClient;
            _verifyCodeAPIClient = verifyCodeAPIClient;
            _correctionMsgQuery = correctionMsgQuery;
            _schoolExtensionQuery = schoolExtensionQuery;
            _schoolRecruitQuery = schoolRecruitQuery;
            _schoolOverViewQuery = schoolOverViewQuery;
            _areaRecruitPlanQuery = areaRecruitPlanQuery;
            _schoolExtensionLevelQuery = schoolExtensionLevelQuery;
            _schoolAchievementQuery = schoolAchievementQuery;
            _schoolQuotaQuery = schoolQuotaQuery;
            _schoolFractionQuery = schoolFractionQuery;
            _commentAPIClient = commentAPIClient;
            _articleAPIClient = articleAPIClient;
            _paidQAAPIClient = paidQAAPIClient;
            _easyRedisClient = easyRedisClient;
            _schoolScoreQuery = schoolScoreQuery;
            _schoolProjectQuery = schoolProjectQuery;
            _userAPIClient = userAPIClient;
            _weChatGatewayService = weChatGatewayService;
            _configuration = configuration;
            _schoolGeneralCommentQuery = schoolGeneralCommentQuery;
        }

        #region 现网院校库广深成重页面迭代
        /// <summary>
        /// 学校详情外页
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="schoolNo">学部No</param>
        /// <modify time="2021-08-31 16:18:07" author="Lonlykids"></modify>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetDetail(Guid eid, string schoolNo = null)
        {
            var result = ResponseResult.Failed();
            var userID = User.Identity.GetID();
            SchoolExtensionDTO schoolInfo = null;
            if (eid == default)
            {
                if (string.IsNullOrWhiteSpace(schoolNo)) return result;
                var int_SchoolNo = UrlShortIdUtil.Base322Long(schoolNo);
                schoolInfo = await _schoolExtensionQuery.GetSchoolExtensionDetails(default, int_SchoolNo);
            }
            else
            {
                schoolInfo = await _schoolExtensionQuery.GetSchoolExtensionDetails(eid, default);
            }

            if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
            {
                result.Msg = "eid not found";
                return result;
            }
            else
            {
                eid = schoolInfo.ExtId;
            }
            var resultData = new GetDetailResponse()
            {
                ShortNo = UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo),
                EID = schoolInfo.ExtId,
                ExtNicknames = schoolInfo.Nickname.FromJsonSafe<IEnumerable<string>>(),
                SchFType = schoolInfo.SchFType.Code
            };

            if (userID != default) resultData.IsCollected = await _userAPIClient.CheckIsCollected(schoolInfo.ExtId, userID);

            var videos = await _schoolExtensionQuery.GetExtensionVideos(eid);
            #region OtherAPI
            var rankingsTask = _articleAPIClient.GetRankingByEID(eid);
            var schoolQuestionsTask = _commentAPIClient.GetQuestionByEID(eid, userID);
            var extArticlesTask = _articleAPIClient.ListByEID(eid);
            var selectedCommentsTask = _commentAPIClient.GetSchoolSelectedComment(eid, userID);
            var orglessonsTask = _articleAPIClient.ListOrgLesson(new Common.OtherAPIClient.Article.Model.ListOrgLessonRequest()
            {
                AreaID = schoolInfo.Area,
                CityID = schoolInfo.City,
                EID = schoolInfo.ExtId,
                Grade = schoolInfo.Grade,
                SchFType = schoolInfo.SchFType.GetDesc(),
                SID = schoolInfo.Sid
            });
            var recommendTalentTask = GetRecommendTalent(schoolInfo.Grade, schoolInfo.ExtId);
            #endregion

            #region 排行相关
            var rankings = await rankingsTask;
            if (rankings?.Any() == true) resultData.Rankings = rankings.Take(3);
            #endregion

            #region 分数相关
            if (schoolInfo.City == 440100) //广州专属
            {
                var scores = await _schoolScoreQuery.GetScores(schoolInfo.ExtId);
                if (scores?.Any() == true)
                {
                    resultData.Scores = new SchoolScore()
                    {
                        Prestige = scores.FirstOrDefault(p => p.IndexID == 25)?.Score / 10,
                        Surround = scores.FirstOrDefault(p => p.IndexID == 21)?.Score / 10,
                        Teaching = (float?)(scores.Where(p => p.IndexID > 14 && p.IndexID < 18).Average(p => p.Score) / 10).CutDoubleWithN(1),
                        StudentSource = scores.FirstOrDefault(p => p.IndexID == 23)?.Score / 10,
                        TotalScore = scores.FirstOrDefault(p => p.IndexID == 22)?.Score / 10
                    };
                    if (resultData.Scores.TotalScore.HasValue)
                    {
                        resultData.Scores.ScoreRanking = (int?)await _schoolScoreQuery.GetSchoolRankingInCity(schoolInfo.City, scores.FirstOrDefault(p => p.IndexID == 22).Score, schoolInfo.SchFType.Code);
                    }
                }
            }
            resultData.ScoreTree = await _schoolScoreQuery.GetSchoolScoreTreeByEID(schoolInfo.ExtId);
            #endregion

            #region  Videos
            if (videos?.Any() == true)
            {
                resultData.SchoolVideo = videos.Select(p => new
                {
                    Time = p.Key.ToString("yyyy-MM-dd"),
                    VideoUrl = p.Value,
                    Type = p.Message,
                    Description = p.Data,
                    CoverImgUrl = p.Other
                });
            }
            #endregion

            #region 招生信息
            var recruits = await _schoolRecruitQuery.GetByEID(eid, type: -1);
            if (recruits?.Any() == true)
            {
                var recruitIDs = recruits.Select(p => p.ID).Distinct();
                var recruitTypes = recruits.Select(p => p.Type).Distinct();
                var recruitDatas = new List<Recruit>();
                var addressEIDs = new List<Guid>();
                foreach (var item in recruits)
                {
                    var recruit = CommonHelper.MapperProperty<OnlineSchoolRecruitInfo, Recruit>(item);
                    IEnumerable<RecruitScheduleInfo> recruitSchedules = null;
                    if (schoolInfo.Grade == (byte)SchoolGradeType.SeniorMiddleSchool)
                    {
                        recruitSchedules = await _schoolRecruitQuery.GetRecruitSchedules(schoolInfo.City, recruitTypes, schoolInfo.SchFType.Code, item.Year);
                    }
                    else
                    {
                        recruitSchedules = await _schoolRecruitQuery.GetRecruitSchedules(schoolInfo.City, recruitTypes, schoolInfo.SchFType.Code, item.Year, schoolInfo.Area);
                    }
                    //招生日程
                    if (recruitSchedules?.Any(p => p.RecruitType == item.Type) == true)
                    {
                        recruit.RecruitSchedules = recruitSchedules.Where(p => p.RecruitType == item.Type).OrderBy(p => p.Index);
                    }
                    if (item.AllocationPrimaryEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.AllocationPrimaryEIDs_Obj);
                    if (item.CounterpartPrimaryEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.CounterpartPrimaryEIDs_Obj);
                    if (item.ScribingScopeEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.ScribingScopeEIDs_Obj);
                    recruitDatas.Add(recruit);
                }
                if (addressEIDs?.Any() == true)
                {
                    addressEIDs = addressEIDs.Distinct().ToList();
                    var addresses = await _schoolExtensionQuery.GetExtAddresses(addressEIDs);
                    if (addresses?.Any() == true)
                    {
                        foreach (var item in recruitDatas)
                        {
                            if (item.AllocationPrimaryEIDs_Obj?.Any() == true)
                            {
                                item.AllocationPrimary = JsonConvert.SerializeObject(addresses.Where(p => item.AllocationPrimaryEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString(),
                                UrlShortIdUtil.Long2Base32(p.Value)
                            }));
                            }
                            if (item.CounterpartPrimaryEIDs_Obj?.Any() == true)
                            {
                                item.CounterpartPrimary = JsonConvert.SerializeObject(addresses.Where(p => item.CounterpartPrimaryEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString(),
                                UrlShortIdUtil.Long2Base32(p.Value)
                            }));
                            }
                            if (item.ScribingScopeEIDs_Obj?.Any() == true)
                            {
                                item.ScribingScope = JsonConvert.SerializeObject(addresses.Where(p => item.ScribingScopeEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString(),
                                UrlShortIdUtil.Long2Base32(p.Value)
                            }));
                            }
                        }
                    }
                }
                resultData.Recruits = recruitDatas.OrderBy(p => p.Type);
            }
            #endregion

            #region 学部其他信息
            var overviewOtherInfo = await _schoolOverViewQuery.GetByEID(eid);
            #endregion

            #region 区域招生政策
            if (schoolInfo.Grade == (byte)SchoolGradeType.SeniorMiddleSchool)
            {
                resultData.AreaRecruitPlan = await _areaRecruitPlanQuery.GetByAreaCodeAndSchFType(schoolInfo.City.ToString(),
                    schoolInfo.SchFType.Code);
            }
            else
            {
                resultData.AreaRecruitPlan = await _areaRecruitPlanQuery.GetByAreaCodeAndSchFType(schoolInfo.Area.ToString(),
                    schoolInfo.SchFType.Code);
            }
            #endregion

            #region 费用相关
            if (resultData.Recruits?.Any(p => !string.IsNullOrWhiteSpace(p.OtherCost) || !string.IsNullOrWhiteSpace(p.ApplyCost) || !string.IsNullOrWhiteSpace(p.Tuition)) == true)
            {
                var find = resultData.Recruits.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.OtherCost) || !string.IsNullOrWhiteSpace(p.ApplyCost) || !string.IsNullOrWhiteSpace(p.Tuition));
                resultData.ApplyCost = find.ApplyCost;
                resultData.Tuition = find.Tuition;
                resultData.OtherCost = find.OtherCost;
            }
            #endregion

            #region 学校问答
            //var schoolQuestion = _questionInfoService.PushSchoolInfo(new List<Guid>() { eid }, userId);
            var schoolQuestions = await schoolQuestionsTask;
            if (schoolQuestions?.Any() == true)
            {
                if (schoolQuestions.First().ID != default)
                {
                    resultData.Question = CommonHelper.MapperProperty<GetQuestionByEIDResponse, QuestionDTO>(schoolQuestions.First(), true);
                    resultData.Question.QuestionCreateTime = schoolQuestions.First().QuestionCreateTime.ToUnixTimestampByMilliseconds();
                }
            }
            #endregion

            #region 分部文章
            //var extArticles = await Task.Run(() =>
            //{
            //    return _articleService.GetComparisionArticles(new List<Guid> { eid }, 2);
            //});
            var extArticles = await extArticlesTask;
            if (extArticles?.Any() == true)
            {
                resultData.ExtArticles = extArticles.Select(p => new
                {
                    p.ID,
                    Time = p.Time.ConciseTime(),
                    p.Layout,
                    p.Title,
                    p.ViewCount,
                    No = UrlShortIdUtil.Long2Base32(p.No)
                });
            }
            #endregion

            #region 其他分部
            var otherExtensions = await _schoolExtensionQuery.GetSchoolExtensionNames(schoolInfo.Sid);
            if (otherExtensions?.Any(p => p.Value != eid) == true)
            {
                resultData.ExtSchools = otherExtensions.Where(p => p.Value != eid);
            }
            #endregion

            #region 机构课程
            var orglessons = await orglessonsTask;
            if (orglessons != default)
            {
                resultData.OrgLessons = new OrgLessonDTO();
                if (orglessons.Lessons?.Any() == true) resultData.OrgLessons.HotSellCourses = orglessons.Lessons.Select(p => CommonHelper.MapperProperty<Common.OtherAPIClient.Article.Model.Entity.HotSellCourse, HotSellCourseInfo>(p, true));
                if (orglessons.Orgs?.Any() == true) resultData.OrgLessons.RecommendOrgs = orglessons.Orgs.Select(p => CommonHelper.MapperProperty<Common.OtherAPIClient.Article.Model.Entity.RecommendOrg, RecommendOrgInfo>(p, true));
            }
            #endregion

            resultData.TypeName = ReplaceSchFTypeName(schoolInfo.SchFType);
            resultData.Grade = schoolInfo.Grade;
            resultData.SchoolName = schoolInfo.SchoolName;
            resultData.ExtName = schoolInfo.ExtName;
            resultData.Latitude = schoolInfo.Latitude;
            resultData.Longitude = schoolInfo.Longitude;
            resultData.EnglishName = schoolInfo.EName;
            resultData.SchoolImageUrl = schoolInfo.SchoolImageUrl;

            #region BrandImages
            if (schoolInfo.SchoolImages?.Any() == true) resultData.SchoolImages = schoolInfo.SchoolImages.ToList();
            #endregion

            #region Tags
            resultData.Tags = new List<string>();
            if (!string.IsNullOrEmpty(resultData.TypeName)) resultData.Tags.Add(resultData.TypeName);
            if (schoolInfo.EduSysType.HasValue) resultData.Tags.Add(CommonHelper.GetDescriptionFromEnumValue(schoolInfo.EduSysType.Value));
            if (schoolInfo.AuthenticationList?.Any() == true)
            {
                var extLevels = await _schoolExtensionLevelQuery.GetByCityCodeSchFType(schoolInfo.City, schoolInfo.SchFType.Code);
                if (extLevels?.Any() == true)
                {
                    var schoolAuths = schoolInfo.AuthenticationList.Select(p => p.Key).Distinct();
                    foreach (var item in extLevels)
                    {
                        if (schoolAuths.Any(p => p == item.ReplaceSource)) resultData.Tags.Add(item.LevelName);
                    }
                }
            }
            #endregion

            #region 对口学校
            if (!string.IsNullOrWhiteSpace(schoolInfo.CounterPart) || schoolInfo.CounterPart != "[]")
            {
                var obj_CounterPart = schoolInfo.CounterPart.FromJsonSafe<IEnumerable<KeyValuePair<string, Guid>>>();
                if (obj_CounterPart?.Any() == true)
                {
                    var eids = obj_CounterPart.Select(p => p.Value).Distinct();
                    var addresses = await _schoolExtensionQuery.GetExtAddresses(eids.ToArray());
                    resultData.CounterPart = addresses.Select(p => (string.Join(" - ",
                        obj_CounterPart.FirstOrDefault(x => x.Value == p.Key).Key.Split('_')), p.Data, p.Key.ToString(), UrlShortIdUtil.Long2Base32(p.Value)));
                }
            }
            #endregion

            #region 派位学校
            if (!string.IsNullOrWhiteSpace(schoolInfo.Allocation) || schoolInfo.Allocation != "[]")
            {
                var obj_Allocation = schoolInfo.Allocation.FromJsonSafe<IEnumerable<KeyValuePair<string, Guid>>>();
                if (obj_Allocation?.Any() == true)
                {
                    var eids = obj_Allocation.Select(p => p.Value).Distinct();
                    var addresses = await _schoolExtensionQuery.GetExtAddresses(eids.ToArray());
                    resultData.Allocation = addresses.Select(p => (string.Join(" - ",
                        obj_Allocation.FirstOrDefault(x => x.Value == p.Key).Key.Split('_')), p.Data, p.Key.ToString(), UrlShortIdUtil.Long2Base32(p.Value)));
                }
            }
            #endregion

            #region OverView
            resultData.Overview = new SchoolOverview()
            {
                Abroad = schoolInfo.Abroad,
                Address = schoolInfo.Address,
                HasCanteen = schoolInfo.Canteen.GetValueOrDefault(),
                Intro = schoolInfo.Intro,
                LodgingType = schoolInfo.LodgingType,
                StudentQuantity = schoolInfo.Studentcount ?? 0,
                TeacherStudentScale = schoolInfo.TsPercent.GetValueOrDefault().ToString(),
                Tel = schoolInfo.Tel,
                WebSite = schoolInfo.WebSite,
                HasSchoolBus = schoolInfo.HasSchoolBus,
                CityCode = schoolInfo.City
            };
            if (schoolInfo.SchFType.Code != "lx432" && schoolInfo.SchFType.Code != "lx430") resultData.Overview.Abroad = null;//国际高中+外籍人员子女高中显示出国方向
            if (overviewOtherInfo != null)
            {
                //招生方式
                resultData.Overview.RecruitWay = overviewOtherInfo.RecruitWay_Obj;
                //resultData.Overview.HasSchoolBus = overviewOtherInfo.HasSchoolBus;
            }
            #endregion

            #region 非幼儿园不能显示手机
            if (!string.IsNullOrWhiteSpace(resultData.Overview.Tel) && schoolInfo.Grade != (byte)SchoolGradeType.Kindergarten)
            {
                var schoolPhone = new List<string>();
                if (resultData.Overview.Tel.Contains('；'))
                {
                    foreach (var item in resultData.Overview.Tel.Split('；'))
                    {
                        if (!CommonHelper.isMobile(item)) schoolPhone.Add(item);
                    }
                }
                else
                {
                    if (!CommonHelper.isMobile(resultData.Overview.Tel)) schoolPhone.Add(resultData.Overview.Tel);
                }
                if (schoolPhone.Any()) resultData.Overview.Tel = string.Join('；', schoolPhone);
            }
            #endregion

            #region 精选点评
            var selectedComments = await selectedCommentsTask;
            if (selectedComments?.Any() == true)
            {
                var selectedComment = selectedComments.First();
                resultData.SeletedComment = new
                {
                    selectedComment.NickName,
                    selectedComment.HeadImgUrl,
                    selectedComment.StartTotal,
                    selectedComment.Content,
                    selectedComment.Images,
                    selectedComment.ReplyCount,
                    selectedComment.LikeCount,
                    CreateTime = selectedComment.CreateTime.ToUnixTimestampByMilliseconds(),
                    selectedComment.ID,
                    selectedComment.ShortCommentNo,
                    selectedComment.IsRumorRefuting,//是否辟谣
                    selectedComment.IsSelected,//是否精选
                    selectedComment.Score?.IsAttend, //是否过来人
                    selectedComment.IsLike,//是否点赞
                    selectedComment.IsTalent,
                    selectedComment.TalentType
                };
            }
            #endregion

            #region 推荐学部
            var recommendExts = await _schoolExtensionQuery.GetRecommendExtension(schoolInfo.Type, schoolInfo.Grade, schoolInfo.City, schoolInfo.ExtId);
            if (recommendExts?.Any() == true)
            {
                resultData.RecommendExtensions = recommendExts.Select(p => new
                {
                    SchoolName = p.Message,
                    ExtName = p.Data,
                    ShortNo = UrlShortIdUtil.Long2Base32(p.Other),
                    EID = p.Value
                });
            }
            #endregion

            #region 升学成绩
            var achievements = await _schoolAchievementQuery.ListByEIDAsync(eid);
            if (achievements?.Any() == true)
            {
                var result_Achievements = new List<Achievement>();
                foreach (var item in achievements)
                {
                    var achievement = CommonHelper.MapperProperty<ExtensionAchievementInfo, Achievement>(item);
                    if (achievement?.Tables_Obj?.Any(p => p.Key == "毕业生去向") == true)
                    {
                        var str_Table = achievement.Tables_Obj.FirstOrDefault(p => p.Key == "毕业生去向").Value;
                        if (!string.IsNullOrWhiteSpace(str_Table))
                        {
                            var htmlDoc = HtmlParser.Query(str_Table);
                            if (htmlDoc("table")?.Any() == true)
                            {
                                var firstTable = htmlDoc("table").First();
                                var forwards = new List<KeyValuePair<string, string>>();
                                foreach (var tr in HtmlParser.Query(firstTable.innerHTML)("tr").Skip(1).Take(3))
                                {
                                    var query_td = HtmlParser.Query(tr.innerHTML)("td");
                                    if (query_td?.Count() > 1)
                                    {
                                        forwards.Add(new KeyValuePair<string, string>(query_td[0].innerHTML, query_td[1].innerHTML));
                                    }
                                }
                                if (forwards?.Any() == true) achievement.Forwards = forwards;
                            }
                        }
                    }
                    result_Achievements.Add(achievement);
                }
                if (result_Achievements?.Any() == true) resultData.Achievements = result_Achievements;
            }
            #endregion
            //指标分配
            resultData.Quotas = await _schoolQuotaQuery.GetByEID(eid);
            //分数线
            resultData.Fractions = await _schoolFractionQuery.ListByEIDAsync(eid);
            //推荐达人
            resultData.Talent = await recommendTalentTask;

            result = ResponseResult.Success(resultData);
            return result;
        }

        static string ReplaceSchFTypeName(SchFType0 schFType)
        {
            return schFType.Code switch
            {
                //公办幼儿园
                "lx110" => "公办幼儿园",
                //普通民办幼儿园
                "lx120" => "民办幼儿园",
                //民办普惠幼儿园
                "lx121" => "民办幼儿园",
                //国际幼儿园
                "lx130" => "国际幼儿园",
                //公办小学
                "lx210" => "公办小学",
                //普通民办小学
                "lx220" => "民办小学",
                //双语小学
                "lx231" => "民办小学",
                //外国人小学
                "lx230" => "外籍人员子女小学",
                //公办初中
                "lx310" => "公办初中",
                //普通民办初中
                "lx320" => "民办初中",
                //双语初中
                "lx331" => "民办初中",
                //外国人初中
                "lx330" => "外籍人员子女初中",
                //公办高中
                "lx410" => "公办高中",
                //普通民办高中
                "lx420" => "民办高中",
                //国际高中
                "lx432" => "国际高中",
                //外国人高中
                "lx430" => "外籍人员子女高中",
                _ => schFType.GetDesc(),
            };
        }
        async Task<TalentDetailExtend> GetRecommendTalent(byte grade, Guid? eid = null)
        {
            var gradeType = 0;
            var random = new Random();
            switch (grade)
            {
                case 1:
                    gradeType = 1;
                    break;
                case 2:
                    gradeType = random.Next(2, 4);
                    break;
                case 3:
                    gradeType = 4;
                    break;
                case 4:
                    gradeType = 5;
                    break;
                default:
                    break;
            }
            if (gradeType < 1) return null;

            return await _paidQAAPIClient.RandomTalentByGrade(grade, true, eid);
        }
        #endregion

        /// <summary>
        /// 学校详情内页
        /// </summary>
        /// <param name="request"></param>
        /// <modify time="2021-09-17 15:12:42" author="Lonlykids"></modify>
        /// <returns>ResponseResult</returns>
        [HttpPost]
        public async Task<ResponseResult> GetInfo(GetInfoRequest request)
        {
            var result = ResponseResult.Failed();
            SchoolExtensionDTO schoolInfo = null;
            if (request.EID == default)
            {
                if (string.IsNullOrWhiteSpace(request.SchoolNo)) return result;
                var int_SchoolNo = UrlShortIdUtil.Base322Long(request.SchoolNo);
                schoolInfo = await _schoolExtensionQuery.GetSchoolExtensionDetails(default, int_SchoolNo);
            }
            else
            {
                schoolInfo = await _schoolExtensionQuery.GetSchoolExtensionDetails(request.EID, default);
            }
            if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
            {
                result.Msg = "eid not found";
                return result;
            }
            var resultData = new GetInfoResponse()
            {
                SchoolID = schoolInfo.Sid,
                ShortNo = UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo),
                EID = schoolInfo.ExtId,
                ExtNicknames = schoolInfo.Nickname.FromJsonSafe<IEnumerable<string>>(),
                SchFType = schoolInfo.SchFType.Code,
                EnglishName = schoolInfo.EName
            };
            var userID = User.Identity.GetID();

            #region loding...
            var recommendExts = await _schoolExtensionQuery.GetRecommendExtension(schoolInfo.Type, schoolInfo.Grade, schoolInfo.City, schoolInfo.ExtId);
            var videos = await _schoolExtensionQuery.GetExtensionVideos(schoolInfo.ExtId);
            var scores = await _schoolScoreQuery.GetScores(schoolInfo.ExtId);//学校分数
            resultData.RecruitYears = await _schoolRecruitQuery.GetYears(schoolInfo.ExtId);
            resultData.Quotas = await _schoolQuotaQuery.GetByEID(schoolInfo.ExtId);
            resultData.QuotaYears = await _schoolQuotaQuery.GetYears(schoolInfo.ExtId);
            resultData.Projects = await _schoolProjectQuery.GetByEID(schoolInfo.ExtId);
            var images = schoolInfo.SchoolImages;
            resultData.CostYears = await _schoolRecruitQuery.GetCostYears(schoolInfo.ExtId);
            #endregion

            #region OtherAPI
            var schoolQuestionsTask = _commentAPIClient.GetQuestionByEID(schoolInfo.ExtId, userID, 3);
            var extArticlesTask = _articleAPIClient.ListByEID(schoolInfo.ExtId);
            var selectedCommentsTask = _commentAPIClient.GetSchoolSelectedComment(schoolInfo.ExtId, userID, 3);
            var orglessonsTask = _articleAPIClient.ListOrgLesson(new Common.OtherAPIClient.Article.Model.ListOrgLessonRequest()
            {
                AreaID = schoolInfo.Area,
                CityID = schoolInfo.City,
                EID = schoolInfo.ExtId,
                Grade = schoolInfo.Grade,
                SchFType = schoolInfo.SchFType.GetDesc(),
                SID = schoolInfo.Sid
            });
            var commentTotalsTask = _commentAPIClient.GetCommentTotals(schoolInfo.Sid);
            var questionTotalsTask = _commentAPIClient.GetQuestionTotals(schoolInfo.ExtId);
            var talentTask = GetRecommendTalent(schoolInfo.Grade, schoolInfo.ExtId);
            #endregion

            #region 招生信息
            var recruits = await _schoolRecruitQuery.GetByEID(schoolInfo.ExtId, type: -1);
            if (recruits?.Any() == true)
            {
                var recruitIDs = recruits.Select(p => p.ID).Distinct();
                var recruitTypes = recruits.Select(p => p.Type).Distinct();
                var recruitDatas = new List<Recruit>();
                var addressEIDs = new List<Guid>();
                foreach (var item in recruits)
                {
                    var recruit = CommonHelper.MapperProperty<OnlineSchoolRecruitInfo, Recruit>(item);
                    //招生日程
                    IEnumerable<RecruitScheduleInfo> recruitSchedules = null;
                    if (schoolInfo.Grade == (byte)SchoolGradeType.SeniorMiddleSchool)
                    {
                        recruitSchedules = await _schoolRecruitQuery.GetRecruitSchedules(schoolInfo.City, recruitTypes, schoolInfo.SchFType.Code, item.Year);
                    }
                    else
                    {
                        recruitSchedules = await _schoolRecruitQuery.GetRecruitSchedules(schoolInfo.City, recruitTypes, schoolInfo.SchFType.Code, item.Year, schoolInfo.Area);
                    }
                    if (recruitSchedules?.Any(p => p.RecruitType == item.Type) == true)
                    {
                        recruit.RecruitSchedules = recruitSchedules.Where(p => p.RecruitType == item.Type).OrderBy(p => p.Index);
                    }
                    if (item.AllocationPrimaryEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.AllocationPrimaryEIDs_Obj);
                    if (item.CounterpartPrimaryEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.CounterpartPrimaryEIDs_Obj);
                    if (item.ScribingScopeEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.ScribingScopeEIDs_Obj);
                    recruitDatas.Add(recruit);
                }
                if (addressEIDs?.Any() == true)
                {
                    addressEIDs = addressEIDs.Distinct().ToList();
                    var addresses = await _schoolExtensionQuery.GetExtAddresses(addressEIDs);
                    if (addresses?.Any() == true)
                    {
                        foreach (var item in recruitDatas)
                        {
                            if (item.AllocationPrimaryEIDs_Obj?.Any() == true)
                            {
                                item.AllocationPrimary = JsonConvert.SerializeObject(addresses.Where(p => item.AllocationPrimaryEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString(),
                                UrlShortIdUtil.Long2Base32(p.Value)
                            }));
                            }
                            if (item.CounterpartPrimaryEIDs_Obj?.Any() == true)
                            {
                                item.CounterpartPrimary = JsonConvert.SerializeObject(addresses.Where(p => item.CounterpartPrimaryEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString(),
                                UrlShortIdUtil.Long2Base32(p.Value)
                            }));
                            }
                            if (item.ScribingScopeEIDs_Obj?.Any() == true)
                            {
                                item.ScribingScope = JsonConvert.SerializeObject(addresses.Where(p => item.ScribingScopeEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString(),
                                UrlShortIdUtil.Long2Base32(p.Value)
                            }));
                            }
                        }
                    }
                }
                resultData.Recruits = recruitDatas.OrderBy(p => p.Type);
            }
            #endregion

            #region Overview
            resultData.Overview = new SchoolOverviewEx()
            {
                Abroad = schoolInfo.Abroad,
                Address = schoolInfo.Address,
                HasCanteen = schoolInfo.Canteen.GetValueOrDefault(),
                HasSchoolBus = schoolInfo.HasSchoolBus,
                Intro = schoolInfo.Intro,
                LodgingType = schoolInfo.LodgingType,
                StudentQuantity = schoolInfo.Studentcount.GetValueOrDefault(),
                TeacherStudentScale = schoolInfo.TsPercent.GetValueOrDefault().ToString(),
                Tel = schoolInfo.Tel,
                WebSite = schoolInfo.WebSite,
                CityCode = schoolInfo.City
            };
            if (schoolInfo.SchFType.Code != "lx432" || schoolInfo.SchFType.Code != "lx430") resultData.Overview.Abroad = null;//国际高中+外籍人员子女高中显示出国方向
            #region 学部其他信息
            var overviewOtherInfo = await _schoolOverViewQuery.GetByEID(schoolInfo.ExtId);
            if (overviewOtherInfo != null)
            {
                // resultData.Overview.HasSchoolBus = overviewOtherInfo.HasSchoolBus;
                //招生方式
                resultData.Overview.RecruitWay = overviewOtherInfo.RecruitWay_Obj;
            }
            #endregion
            #endregion

            #region CounterParts
            if (!string.IsNullOrWhiteSpace(schoolInfo.CounterPart) || schoolInfo.CounterPart != "[]")
            {
                try
                {
                    var obj_CounterPart = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, Guid>>>(schoolInfo.CounterPart);
                    var eids = obj_CounterPart.Select(p => p.Value).Distinct();
                    var addresses = await _schoolExtensionQuery.GetExtAddresses(eids.ToArray());
                    resultData.CounterParts = addresses.Select(p => (string.Join(" - ",
                        obj_CounterPart.FirstOrDefault(x => x.Value == p.Key).Key.Split('_')), p.Data, p.Key.ToString(), UrlShortIdUtil.Long2Base32(p.Value)));
                }
                catch { }
            }
            resultData.CounterPartYears = await _schoolExtensionQuery.GetSchoolFieldYears(schoolInfo.ExtId, "CounterPart");
            #endregion

            #region 派位学校
            if (!string.IsNullOrWhiteSpace(schoolInfo.Allocation) || schoolInfo.Allocation != "[]")
            {
                var obj_Allocation = schoolInfo.Allocation.FromJsonSafe<IEnumerable<KeyValuePair<string, Guid>>>();
                if (obj_Allocation?.Any() == true)
                {
                    var eids = obj_Allocation.Select(p => p.Value).Distinct();
                    var addresses = await _schoolExtensionQuery.GetExtAddresses(eids.ToArray());
                    resultData.Allocations = addresses.Select(p => (string.Join(" - ",
                        obj_Allocation.FirstOrDefault(x => x.Value == p.Key).Key.Split('_')), p.Data, p.Key.ToString(), UrlShortIdUtil.Long2Base32(p.Value)));
                }
                resultData.AllocationYears = await _schoolExtensionQuery.GetSchoolFieldYears(schoolInfo.ExtId, "Allocation");
            }
            #endregion

            #region  Videos
            if (videos?.Any() == true)
            {
                resultData.Videos = videos.Select(p => new
                {
                    Time = p.Key.ToString("yyyy-MM-dd"),
                    VideoUrl = p.Value,
                    Type = p.Message,
                    Description = p.Data,
                    CoverImgUrl = p.Other
                });
            }
            #endregion

            #region Images
            if (schoolInfo.SchoolImages?.Any() == true)
            {
                resultData.Images = schoolInfo.SchoolImages;
            }
            #endregion

            #region 推荐达人
            resultData.Talent = await talentTask;
            #endregion

            #region 其他学部
            var otherExtensions = await _schoolExtensionQuery.GetSchoolExtensionNames(schoolInfo.Sid);
            if (otherExtensions?.Any(p => p.Value != schoolInfo.ExtId) == true)
            {
                resultData.OtherExt = otherExtensions.Where(p => p.Value != schoolInfo.ExtId);
            }
            #endregion

            resultData.TypeName = ReplaceSchFTypeName(schoolInfo.SchFType);
            resultData.SchoolName = schoolInfo.SchoolName;
            resultData.SchoolExtName = schoolInfo.ExtName;
            resultData.Grade = schoolInfo.Grade;
            resultData.Longitude = schoolInfo.Longitude;
            resultData.Latitude = schoolInfo.Latitude;

            #region 区域招生政策
            if (schoolInfo.Grade == (byte)SchoolGradeType.SeniorMiddleSchool)
            {
                resultData.AreaRecruitPlans = await _areaRecruitPlanQuery.GetByAreaCodeAndSchFType(schoolInfo.City.ToString(),
                    schoolInfo.SchFType.Code);
                resultData.AreaRecruitPlanYears = await _areaRecruitPlanQuery.GetYears(schoolInfo.City.ToString(),
                schoolInfo.SchFType.Code);
            }
            else
            {
                resultData.AreaRecruitPlans = await _areaRecruitPlanQuery.GetByAreaCodeAndSchFType(schoolInfo.Area.ToString(),
                    schoolInfo.SchFType.Code);
                resultData.AreaRecruitPlanYears = await _areaRecruitPlanQuery.GetYears(schoolInfo.Area.ToString(),
                schoolInfo.SchFType.Code);
            }
            #endregion

            #region 收费标准
            if (resultData.Recruits?.Any(p => !string.IsNullOrWhiteSpace(p.OtherCost) || !string.IsNullOrWhiteSpace(p.ApplyCost) ||
            !string.IsNullOrWhiteSpace(p.Tuition)) == true)
            {
                var find = resultData.Recruits.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.OtherCost) || !string.IsNullOrWhiteSpace(p.ApplyCost) || !string.IsNullOrWhiteSpace(p.Tuition));
                resultData.ApplyCost = find.ApplyCost;
                resultData.Tuition = find.Tuition;
                resultData.OtherCost = find.OtherCost;
            }
            #endregion

            #region Courses
            if (schoolInfo.CoursesList?.Any() == true) resultData.Courses = schoolInfo.CoursesList.Select(p => p.Key);
            if (!string.IsNullOrWhiteSpace(schoolInfo.CourseCharacteristic)) resultData.CourseCharacteristic = schoolInfo.CourseCharacteristic;
            if (!string.IsNullOrWhiteSpace(schoolInfo.CourseAuthentication))
            {
                try
                {
                    var courseAuth_Obj = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(schoolInfo.CourseAuthentication);
                    if (courseAuth_Obj?.Any() == true)
                    {
                        resultData.CourseAuths = courseAuth_Obj.Select(p => p.Key);
                    }
                }
                catch { }
            }
            #endregion

            #region 学校认证
            if (overviewOtherInfo?.Certifications_Obj != null) resultData.SchoolAuths = overviewOtherInfo.Certifications_Obj;
            #endregion

            #region Fractions
            resultData.Fractions = await _schoolFractionQuery.ListByEIDAsync(schoolInfo.ExtId);
            resultData.FractionYears = await _schoolFractionQuery.GetYearsAsync(schoolInfo.ExtId);
            #endregion

            #region 升学成绩
            resultData.Achievements = await _schoolAchievementQuery.ListByEIDAsync(schoolInfo.ExtId);
            resultData.AchievemenYears = await _schoolAchievementQuery.GetYears(schoolInfo.ExtId);
            #endregion

            #region 分部文章
            var extArticles = await extArticlesTask;
            if (extArticles?.Any() == true)
            {
                resultData.ExtArticles = extArticles.Select(p => new
                {
                    p.ID,
                    Time = p.Time.ConciseTime(),
                    p.Layout,
                    p.Title,
                    p.ViewCount,
                    No = UrlShortIdUtil.Long2Base32(p.No)
                });
            }
            #endregion

            #region 机构课程
            var orglessons = await orglessonsTask;
            if (orglessons != default)
            {
                resultData.OrgLessons = new OrgLessonDTO();
                if (orglessons.Lessons?.Any() == true) resultData.OrgLessons.HotSellCourses = orglessons.Lessons.Select(p => CommonHelper.MapperProperty<Common.OtherAPIClient.Article.Model.Entity.HotSellCourse, HotSellCourseInfo>(p, true));
                if (orglessons.Orgs?.Any() == true) resultData.OrgLessons.RecommendOrgs = orglessons.Orgs.Select(p => CommonHelper.MapperProperty<Common.OtherAPIClient.Article.Model.Entity.RecommendOrg, RecommendOrgInfo>(p, true));
            }
            #endregion

            #region 非幼儿园不能显示手机
            if (!string.IsNullOrWhiteSpace(resultData.Overview.Tel) && schoolInfo.Grade != (byte)SchoolGradeType.Kindergarten)
            {
                var schoolPhone = new List<string>();
                if (resultData.Overview.Tel.Contains('；'))
                {
                    foreach (var item in resultData.Overview.Tel.Split('；'))
                    {
                        if (!CommonHelper.isMobile(item)) schoolPhone.Add(item);
                    }
                }
                else
                {
                    if (!CommonHelper.isMobile(resultData.Overview.Tel)) schoolPhone.Add(resultData.Overview.Tel);
                }
                if (schoolPhone.Any()) resultData.Overview.Tel = string.Join('；', schoolPhone);
            }
            #endregion

            #region 推荐学部
            if (recommendExts?.Any() == true)
            {
                resultData.RecommendExtensions = recommendExts.Select(p => new
                {
                    SchoolName = p.Message,
                    ExtName = p.Data,
                    ShortNo = UrlShortIdUtil.Long2Base32(p.Other),
                    EID = p.Value
                });
            }
            #endregion

            #region 点评相关
            var commentTotals = await commentTotalsTask;
            if (commentTotals?.Any() == true)
            {
                foreach (var item in commentTotals.Where(p => p.SchoolSectionID != default))
                {
                    item.TotalTypeName = otherExtensions?.FirstOrDefault(x => x.Value == item.SchoolSectionID)?.Key;
                }
                if (scores?.Any(p => p.IndexID == 22) == true)
                {
                    resultData.CommentTags = new
                    {
                        SchoolStarts = scores.FirstOrDefault(p => p.IndexID == 22)?.Score / 20,
                        commentTotals
                    };
                }
                else
                {
                    resultData.CommentTags = new { commentTotals };
                }
            }

            #region 精选点评
            var selectedComments = await selectedCommentsTask;
            if (selectedComments?.Any() == true) resultData.SeletedComments = selectedComments.Select(selectedComment => new
            {
                selectedComment.NickName,
                selectedComment.HeadImgUrl,
                selectedComment.StartTotal,
                selectedComment.Content,
                selectedComment.Images,
                selectedComment.ReplyCount,
                selectedComment.LikeCount,
                CreateTime = selectedComment.CreateTime.ToUnixTimestampByMilliseconds(),
                selectedComment.ID,
                selectedComment.ShortCommentNo,
                selectedComment.IsRumorRefuting,//是否辟谣
                selectedComment.IsSelected,//是否精选
                selectedComment.Score?.IsAttend, //是否过来人
                selectedComment.IsLike,//是否点赞
                selectedComment.IsTalent,
                selectedComment.TalentType
            });
            #endregion
            #endregion

            #region 问答相关
            var questionTotals = await questionTotalsTask;
            if (questionTotals?.Any() == true)
            {
                foreach (var item in questionTotals.Where(p => p.SchoolSectionID != default))
                {
                    item.TotalTypeName = otherExtensions?.FirstOrDefault(x => x.Value == item.SchoolSectionID)?.Key;
                }
                if (scores?.Any(p => p.IndexID == 22) == true)
                {
                    resultData.QuestionTags = new
                    {
                        SchoolStarts = scores.First(p => p.IndexID == 22)?.Score / 20,
                        questionTotals
                    };
                }
                else
                {
                    resultData.QuestionTags = new { questionTotals };
                }
            }

            var schoolQuestions = await schoolQuestionsTask;
            if (schoolQuestions?.Any() == true)
            {
                resultData.SeletedQuestions = schoolQuestions.Select(p =>
                {
                    var find = CommonHelper.MapperProperty<GetQuestionByEIDResponse, QuestionDTO>(p, true);
                    find.QuestionCreateTime = p.QuestionCreateTime.ToUnixTimestampByMilliseconds();
                    return find;
                });
            }
            #endregion

            #region Tags
            resultData.Tags = new List<string>();
            if (!string.IsNullOrEmpty(resultData.TypeName)) resultData.Tags.Add(resultData.TypeName);
            if (schoolInfo.EduSysType.HasValue) resultData.Tags.Add(CommonHelper.GetDescriptionFromEnumValue(schoolInfo.EduSysType.Value));
            if (schoolInfo.AuthenticationList?.Any() == true)
            {
                var extLevels = await _schoolExtensionLevelQuery.GetByCityCodeSchFType(schoolInfo.City, schoolInfo.SchFType.Code);
                if (extLevels?.Any() == true)
                {
                    var schoolAuths = schoolInfo.AuthenticationList.Select(p => p.Key).Distinct();
                    foreach (var item in extLevels)
                    {
                        if (schoolAuths.Any(p => p == item.ReplaceSource)) resultData.Tags.Add(item.LevelName);
                    }
                }
            }
            #endregion

            #region 雷达图
            resultData.ScoreTree = await _schoolScoreQuery.GetSchoolScoreTreeByEID(schoolInfo.ExtId);
            #endregion

            result = ResponseResult.Success(resultData);

            return result;
        }

        /// <summary>
        /// 获取招生信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetRecruits(GetRecruitsRequest request)
        {
            var result = ResponseResult.Failed("param error or no data");
            if (request == null || request.EID == default) return result;
            var schoolInfo = await _schoolExtensionQuery.GetSchoolExtensionDetails(request.EID, default);
            if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
            {
                result.Msg = "eid not found";
                return result;
            }
            var recruits = await _schoolRecruitQuery.GetByEID(request.EID, request.Year ?? 0, request.Type ?? -1);
            if (recruits?.Any() == true)
            {
                var recruitIDs = recruits.Select(p => p.ID).Distinct();
                var recruitTypes = recruits.Select(p => p.Type).Distinct();
                var recruitDatas = new List<Recruit>();
                var addressEIDs = new List<Guid>();
                foreach (var item in recruits)
                {
                    var recruit = CommonHelper.MapperProperty<OnlineSchoolRecruitInfo, Recruit>(item);
                    //招生日程
                    IEnumerable<RecruitScheduleInfo> recruitSchedules = null;
                    if (schoolInfo.Grade == (byte)SchoolGradeType.SeniorMiddleSchool)
                    {
                        recruitSchedules = await _schoolRecruitQuery.GetRecruitSchedules(schoolInfo.City, recruitTypes, schoolInfo.SchFType.Code, item.Year);
                    }
                    else
                    {
                        recruitSchedules = await _schoolRecruitQuery.GetRecruitSchedules(schoolInfo.City, recruitTypes, schoolInfo.SchFType.Code, item.Year, schoolInfo.Area);
                    }
                    if (recruitSchedules?.Any(p => p.RecruitType == item.Type) == true)
                    {
                        recruit.RecruitSchedules = recruitSchedules.Where(p => p.RecruitType == item.Type).OrderBy(p => p.Index);
                    }
                    if (item.AllocationPrimaryEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.AllocationPrimaryEIDs_Obj);
                    if (item.CounterpartPrimaryEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.CounterpartPrimaryEIDs_Obj);
                    if (item.ScribingScopeEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.ScribingScopeEIDs_Obj);
                    recruitDatas.Add(recruit);
                }
                if (addressEIDs?.Any() == true)
                {
                    addressEIDs = addressEIDs.Distinct().ToList();
                    var addresses = await _schoolExtensionQuery.GetExtAddresses(addressEIDs);
                    if (addresses?.Any() == true)
                    {
                        foreach (var item in recruitDatas)
                        {
                            if (item.AllocationPrimaryEIDs_Obj?.Any() == true)
                            {
                                item.AllocationPrimary = JsonConvert.SerializeObject(addresses.Where(p => item.AllocationPrimaryEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString(),
                                UrlShortIdUtil.Long2Base32(p.Value)
                            }));
                            }
                            if (item.CounterpartPrimaryEIDs_Obj?.Any() == true)
                            {
                                item.CounterpartPrimary = JsonConvert.SerializeObject(addresses.Where(p => item.CounterpartPrimaryEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString(),
                                UrlShortIdUtil.Long2Base32(p.Value)
                            }));
                            }
                            if (item.ScribingScopeEIDs_Obj?.Any() == true)
                            {
                                item.ScribingScope = JsonConvert.SerializeObject(addresses.Where(p => item.ScribingScopeEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString(),
                                UrlShortIdUtil.Long2Base32(p.Value)
                            }));
                            }
                        }
                    }
                }
                result = ResponseResult.Success(recruitDatas.OrderBy(p => p.Type));
            }
            return result;
        }

        /// <summary>
        /// 换取短ID
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetSchoolNo(GetSchoolNoRequest request)
        {
            var result = ResponseResult.Failed("param error or no data");
            if (request == null || request.EID == default) return result;
            var finds = await _schoolExtensionQuery.GetSchoolNosAsync(new Guid[] { request.EID });
            if (finds?.Any(p => p.EID == request.EID) == true)
            {
                result = ResponseResult.Success(new
                {
                    ShortSchoolNo = finds.FirstOrDefault(p => p.EID == request.EID).Base32String
                });
            }
            return result;
        }

        /// <summary>
        /// 获取对口学校
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetCounterPart(GetCounterPartRequest request)
        {
            var result = ResponseResult.Failed("param error or no data");
            if (request == null || request.EID == default) return result;
            var resultData = new GetCounterPartResponse();
            if (!request.Year.HasValue)
            {
                var schoolInfo = await _schoolExtensionQuery.GetSchoolExtensionDetails(request.EID, default);
                if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
                {
                    result.Msg = "eid not found";
                    return result;
                }
                if (!string.IsNullOrWhiteSpace(schoolInfo.CounterPart) || schoolInfo.CounterPart != "[]")
                {
                    try
                    {
                        var obj_CounterPart = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, Guid>>>(schoolInfo.CounterPart);
                        var eids = obj_CounterPart.Select(p => p.Value).Distinct();
                        var addresses = await _schoolExtensionQuery.GetExtAddresses(eids.ToArray());
                        resultData.CounterPart = addresses.Select(p => (string.Join(" - ",
                            obj_CounterPart.FirstOrDefault(x => x.Value == p.Key).Key.Split('_')), p.Data, p.Key.ToString(), UrlShortIdUtil.Long2Base32(p.Value)));
                    }
                    catch { }
                }
                resultData.Years = await _schoolExtensionQuery.GetSchoolFieldYears(request.EID, "CounterPart");
                result = ResponseResult.Success(resultData);
            }
            else
            {
                var find = await _schoolExtensionQuery.GetSchoolFieldContentAsync(request.EID, "CounterPart", request.Year.Value);
                if (!string.IsNullOrWhiteSpace(find) || find != "[]")
                {
                    try
                    {
                        var obj_CounterPart = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, Guid>>>(find);
                        var eids = obj_CounterPart.Select(p => p.Value).Distinct();
                        var addresses = await _schoolExtensionQuery.GetExtAddresses(eids.ToArray());
                        resultData.CounterPart = addresses.Select(p => (string.Join(" - ",
                            obj_CounterPart.FirstOrDefault(x => x.Value == p.Key).Key.Split('_')), p.Data, p.Key.ToString(), UrlShortIdUtil.Long2Base32(p.Value)));
                    }
                    catch { }
                }
                result = ResponseResult.Success(resultData);
            }
            return result;
        }

        /// <summary>
        /// 获取派位学校
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetAllocation(GetAllocationRequest request)
        {
            var result = ResponseResult.Failed("param error or no data");
            if (request == null || request.EID == default) return result;
            var resultData = new GetAllocationResponse();
            if (!request.Year.HasValue)
            {
                var schoolInfo = await _schoolExtensionQuery.GetSchoolExtensionDetails(request.EID, default);
                if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
                {
                    result.Msg = "eid not found";
                    return result;
                }
                if (!string.IsNullOrWhiteSpace(schoolInfo.Allocation) || schoolInfo.Allocation != "[]")
                {
                    try
                    {
                        var obj_CounterPart = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, Guid>>>(schoolInfo.Allocation);
                        var eids = obj_CounterPart.Select(p => p.Value).Distinct();
                        var addresses = await _schoolExtensionQuery.GetExtAddresses(eids.ToArray());
                        resultData.Allocations = addresses.Select(p => (string.Join(" - ",
                            obj_CounterPart.FirstOrDefault(x => x.Value == p.Key).Key.Split('_')), p.Data, p.Key.ToString(), UrlShortIdUtil.Long2Base32(p.Value)));
                    }
                    catch { }
                }
                resultData.Years = await _schoolExtensionQuery.GetSchoolFieldYears(request.EID, "Allocation");
                result = ResponseResult.Success(resultData);
            }
            else
            {
                var find = await _schoolExtensionQuery.GetSchoolFieldContentAsync(request.EID, "Allocation", request.Year.Value);
                if (!string.IsNullOrWhiteSpace(find) || find != "[]")
                {
                    try
                    {
                        var obj_CounterPart = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, Guid>>>(find);
                        var eids = obj_CounterPart.Select(p => p.Value).Distinct();
                        var addresses = await _schoolExtensionQuery.GetExtAddresses(eids.ToArray());
                        resultData.Allocations = addresses.Select(p => (string.Join(" - ",
                            obj_CounterPart.FirstOrDefault(x => x.Value == p.Key).Key.Split('_')), p.Data, p.Key.ToString(), UrlShortIdUtil.Long2Base32(p.Value)));
                    }
                    catch { }
                }
                result = ResponseResult.Success(resultData);
            }
            return result;
        }

        /// <summary>
        /// 获取指标分配
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetQuotas(GetQuotasRequest request)
        {
            var result = ResponseResult.Failed("param error or no data");
            if (request == null || request.EID == default) return result;
            var finds = await _schoolQuotaQuery.GetByEID(request.EID, request.Year ?? 0, request.Type ?? 0);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(finds);
            }
            return result;
        }

        /// <summary>
        /// 获取分数线
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetFractions(GetFractionsRequest request)
        {
            var result = ResponseResult.Failed("param error or no data");
            if (request == null || request.EID == default) return result;
            var finds = await _schoolFractionQuery.ListByEIDAsync(request.EID, request.Year ?? 0, request.Type);
            if (finds?.Any() == true) result = ResponseResult.Success(finds);
            return result;
        }
        [HttpPost]
        public async Task<ResponseResult> GetFractions2(GetFractions2Request request)
        {
            var result = ResponseResult.Failed("param error or no data");
            if (request == null || request.EID == default) return result;
            var finds = await _schoolFractionQuery.Get2ByEID(request.EID, request.Year ?? 0, request.Type ?? 0);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(finds);
            }
            return result;
        }
        /// <summary>
        /// 获取升学成绩
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetAchievements(GetAchievementsRequest request)
        {
            var result = ResponseResult.Failed("param error or no data");
            if (request == null || request.EID == default) return result;
            var finds = await _schoolAchievementQuery.ListByEIDAsync(request.EID, request.Year ?? 0);
            if (finds?.Any() == true) result = ResponseResult.Success(finds);
            return result;
        }

        /// <summary>
        /// 获取区域招生政策
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetAreaRecruitPlans(GetAreaRecruitPlansRequest request)
        {
            var result = ResponseResult.Failed("param error or no data");
            if (request == null || request.EID == default) return result;
            var schoolInfo = await _schoolExtensionQuery.GetSchoolExtensionDetails(request.EID, default);
            if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
            {
                result.Msg = "eid not found";
                return result;
            }
            IEnumerable<AreaRecruitPlanInfo> resultDatas;
            if (schoolInfo.Grade == (byte)SchoolGradeType.SeniorMiddleSchool)
            {
                resultDatas = await _areaRecruitPlanQuery.GetByAreaCodeAndSchFType(schoolInfo.City.ToString(),
                    schoolInfo.SchFType.Code, request.Year ?? 0);
            }
            else
            {
                resultDatas = await _areaRecruitPlanQuery.GetByAreaCodeAndSchFType(schoolInfo.Area.ToString(),
                    schoolInfo.SchFType.Code, request.Year ?? 0);
            }
            if (resultDatas?.Any() == true) result = ResponseResult.Success(resultDatas);
            return result;
        }
        [HttpPost]
        public async Task<ResponseResult> GetCostByYear(GetCostByYearRequest request)
        {
            var result = ResponseResult.Failed("param error or no data");
            if (request == null || request.EID == default || request.Year < 1900) return result;
            var finds = await _schoolRecruitQuery.GetCostByYearAsync(request.EID, request.Year);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(new
                {
                    finds.First().Tuition,
                    finds.First().ApplyCost,
                    finds.First().OtherCost_Obj
                });
            }
            return result;
        }

        /// <summary>
        /// 发送纠错信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> PostCorrectionMsg(PostCorrectionMsgRequest request)
        {
            var result = ResponseResult.Failed(ResponseCode.ValidationError, default);
            if (request == null) return result;
            if (!await _verifyCodeAPIClient.VerifyRndCodeAsync(request.Mobile, request.CodeType, request.Code))
            {
                result.Msg = "验证码错误";
                return result;
            }
            var entity = new CorrectionMessageInfo()
            {
                Content = request.Content,
                IdentityType = request.IdentityType,
                Mobile = request.Mobile,
                Nickname = request.Nickname
            };
            if (await _correctionMsgQuery.InsertAsync(entity))
            {
                var userIDs = new string[] { "ChenSiYuan", "Rui", "HeQiQi", "LiuGang", "LaiQingShan", "HuangYingYi", "YaoMiaoDi", "YeCuiLian" };
                if (CommonHelper.IsDev()) userIDs = new string[] { "HaoFang", "YangGuoQiang", "YanHanJiao", "QiuZhengYang" };
                var sb = new StringBuilder();
                sb.AppendLine($"昵称：{entity.Nickname}");
                sb.AppendLine($"手机号码：{entity.Mobile}");
                sb.AppendLine($"身份：{entity.IdentityType.Description()}");
                sb.AppendLine($"内容：{Environment.NewLine + entity.Content}");
                _ = _wxWorkAPIClient.SendWxWorkMsgAsync(userIDs, sb.ToString(), "1000017");
                result = ResponseResult.Success();
            }
            return result;
        }

        [HttpPost]
        public async Task<ResponseResult> GetExtSimpleInfo(GetExtSimpleInfoRequest request)
        {
            var result = ResponseResult.DefaultFailed();
            if (request == default || request.EIDs == default || !request.EIDs.Any()) return result;
            var finds = await _schoolExtensionQuery.GetExtSimpleInfoAsync(request.EIDs);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(finds);
            }
            return result;
        }

        /// <summary>
        /// 根据EID获取推荐的学部
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> ListRecommendExtByEID(Guid eid)
        {
            var result = ResponseResult.DefaultFailed();
            var finds = await _schoolExtensionQuery.ListRecommendSchoolAsync(eid);
            if (finds?.Any() == true) result = ResponseResult.Success(finds.Select(p => new
            {
                Name = $"{p.SchoolName}-{p.ExtName}",
                SNO = UrlShortIdUtil.Long2Base32(p.ExtNo)
            }));

            return result;
        }

        /// <summary>
        /// 获取订阅学部二维码
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetSubscribeQRCode(Guid eid)
        {
            var result = ResponseResult.Failed();
            if (eid == default) return result;
            var callbakcUrl = _configuration.GetSection("RootPath").Value;
#if DEBUG
            callbakcUrl = "https://sxb.lonlykids.com:38443";
#endif
            callbakcUrl += "/School/SubscribeCallback";
            string qrcodeImgUrl = await _weChatGatewayService.GetSenceQRCode(new Infrastructures.Services.Models.GetSenceQRCodeRequest()
            {
                app = 0,
                callBackUrl = callbakcUrl,
                expireSecond = 600,//10 mins
                attach = eid.ToString(),
                fw = "ExtensionSubscribe"

            });
            if (!string.IsNullOrWhiteSpace(qrcodeImgUrl)) result = ResponseResult.Success(qrcodeImgUrl);
            return result;
        }

        [HttpPost]
        public async Task<ResponseResult> SubscribeCallback(WPScanCallBackData callBackData)
        {
            var result = ResponseResult.Failed();
            if (callBackData == default || string.IsNullOrWhiteSpace(callBackData.Attach) || callBackData.UserId == default) return result;
            if (Guid.TryParse(callBackData.Attach, out Guid eid))
            {
                if (await _schoolExtensionQuery.SubscribeAsync(eid, callBackData.UserId))
                {
                    await _weChatGatewayService.SendSendTextMsg(callBackData.OpenId, $"订阅成功！学校的信息将第一时间通过公众号推送给你");
                    result = ResponseResult.Success();
                }
            }
            return result;
        }

        /// <summary>
        /// 获取学部周边信息
        /// <para>TypeCode</para>
        /// <para>
        /// 其他=0
        /// 商场=1
        /// 书店=2
        /// 医院=3
        /// 警察局=4
        /// 地铁站=5
        /// 公交站=6
        /// </para>
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="shortNo">学部短ID</param>
        /// <param name="returnAllBuildingImgs"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetExtSurroundInfos(Guid eid, string shortNo, bool returnAllBuildingImgs = false)
        {
            var result = ResponseResult.DefaultFailed();
            long schoolNo = 0;
            if (!string.IsNullOrWhiteSpace(shortNo))
            {
                schoolNo = UrlShortIdUtil.Base322Long(shortNo);
                if (schoolNo > 0) eid = default;
            }
            var schoolInfo = await _schoolExtensionQuery.GetSchoolExtensionDetails(eid, schoolNo);
            if (schoolInfo == default) return result;
            if (!schoolInfo.Latitude.HasValue || !schoolInfo.Longitude.HasValue) return result;
            #region Tags
            var tags = new List<string>();

            var extTypeName = ReplaceSchFTypeName(schoolInfo.SchFType);

            if (!string.IsNullOrEmpty(extTypeName)) tags.Add(extTypeName);
            if (schoolInfo.EduSysType.HasValue) tags.Add(CommonHelper.GetDescriptionFromEnumValue(schoolInfo.EduSysType.Value));
            if (schoolInfo.AuthenticationList?.Any() == true)
            {
                var extLevels = await _schoolExtensionLevelQuery.GetByCityCodeSchFType(schoolInfo.City, schoolInfo.SchFType.Code);
                if (extLevels?.Any() == true)
                {
                    var schoolAuths = schoolInfo.AuthenticationList.Select(p => p.Key).Distinct();
                    foreach (var item in extLevels)
                    {
                        if (schoolAuths.Any(p => p == item.ReplaceSource)) tags.Add(item.LevelName);
                    }
                }
            }
            #endregion

            var surroundInfo = await _schoolExtensionQuery.ListExtSurroundInfosAsync(schoolInfo.Longitude.Value, schoolInfo.Latitude.Value);
            var buildings = await _schoolExtensionQuery.PageExtSurroundBuildingsAsync(schoolInfo.Longitude.Value, schoolInfo.Latitude.Value);
            if (surroundInfo?.Any() == true || buildings?.Any() == true)
            {
                dynamic resultData = new
                {
                    schoolInfo.SchoolName,
                    schoolInfo.ExtName,
                    schoolInfo.EName,
                    Tags = tags,
                    Surround = surroundInfo?.OrderBy(p => p.TypeCode).ThenBy(p => p.Distance).Select(p => new
                    {
                        Type = p.TypeCode,
                        Name = p.Poi_Name,
                        p.Distance,
                        Attributes = p.Poi_Address?.FromJsonSafe<dynamic>(),
                        Images = p.Poi_Photo?.FromJsonSafe<dynamic>(),
                        Tags = p.Poi_Tags.FromJsonSafe<string[]>().Distinct()
                    }),
                    SurroundCounts = (await _schoolExtensionQuery.ListSurroundCountAsync(schoolInfo.Longitude.Value, schoolInfo.Latitude.Value))?.Select(p => new
                    {
                        p.TypeCode,
                        p.Count
                    }),
                    Buildings = buildings?.Where(p => p.ID != default).Select(p =>
                    {
                        var images = string.IsNullOrWhiteSpace(p.New_House_Img) ? p.House_Img?.FromJsonSafe<string[]>() : p.New_House_Img?.FromJsonSafe<string[]>();
                        var item = new
                        {
                            p.Name,
                            Price = p.House_Price,
                            p.Address,
                            p.ID,
                            p.Distance,
                            Images = images?.Any() == true ? returnAllBuildingImgs ? images : images.Take(1) : null
                        };
                        return item;
                    }).OrderBy(p => p.Distance),
                    BuildingCount = buildings?.Any() == true ? await _schoolExtensionQuery.GetSurroundBuildingCountAsync(schoolInfo.Longitude.Value, schoolInfo.Latitude.Value) : 0
                };
                result = ResponseResult.Success(resultData);
            }
            return result;
        }

        /// <summary>
        /// 分页获取学部周边信息
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="shortNo">学部短ID</param>
        /// <param name="typeCode">类型</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> PageExtSurroundInfo(Guid eid, string shortNo, int typeCode, int pageIndex = 2, int pageSize = 5)
        {
            var result = ResponseResult.DefaultFailed();
            long schoolNo = 0;
            if (!string.IsNullOrWhiteSpace(shortNo))
            {
                schoolNo = UrlShortIdUtil.Base322Long(shortNo);
                if (schoolNo > 0) eid = default;
            }
            var schoolInfo = await _schoolExtensionQuery.GetSchoolExtensionDetails(eid, schoolNo);
            if (schoolInfo == default) return result;
            if (!schoolInfo.Latitude.HasValue || !schoolInfo.Longitude.HasValue) return result;
            var surroundInfo = await _schoolExtensionQuery.PageExtSurroundInfosAsync(schoolInfo.Longitude.Value, schoolInfo.Latitude.Value, pageIndex: pageIndex, pageSize: pageSize, typeCode: typeCode);
            if (surroundInfo?.Any() == true)
            {
                result = ResponseResult.Success(surroundInfo.OrderBy(p => p.Distance).Select(p => new
                {
                    Type = p.TypeCode,
                    Name = p.Poi_Name,
                    p.Distance,
                    Attributes = p.Poi_Address?.FromJsonSafe<dynamic>(),
                    Images = p.Poi_Photo?.FromJsonSafe<dynamic>(),
                    Tags = p.Poi_Tags.FromJsonSafe<string[]>().Distinct()
                }));
            }
            return result;
        }

        /// <summary>
        /// 获取房产周边学部
        /// </summary>
        /// <param name="id">房产ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetSurroundExts(Guid id)
        {
            var result = ResponseResult.DefaultFailed();
            if (id == default) return result;
            var buildings = await _schoolExtensionQuery.ListSurroundBuildingsAsync(new Guid[] { id });
            if (buildings?.Any() == true)
            {
                var building = buildings.First();
                var eids = await _schoolExtensionQuery.ListSurroundExtIDsAsync(building.House_Lng.GetValueOrDefault(), building.House_Lat.GetValueOrDefault(), new SchoolGradeType[] {
                    SchoolGradeType.Kindergarten,
                    SchoolGradeType.PrimarySchool,
                    SchoolGradeType.JuniorMiddleSchool,
                    SchoolGradeType.SeniorMiddleSchool
                });
                if (eids?.Any() == true)
                {
                    var exts = await _schoolExtensionQuery.ListExtInfoForSurroundAsync(eids.Select(p => p.EID));
                    if (exts?.Any() == true)
                    {
                        var resps = exts.Select(p =>
                        {
                            var item = CommonHelper.MapperProperty<ExtSimpleDTO, ExtInfo>(p);
                            var tags = new List<string>();
                            var extTypeName = ReplaceSchFTypeName(new SchFType0(p.SchFType));
                            if (!string.IsNullOrWhiteSpace(extTypeName)) tags.Add(extTypeName);
                            if (!string.IsNullOrWhiteSpace(item.EduSysType.GetDescription())) tags.Add(item.EduSysType.GetDescription());
                            if (!string.IsNullOrWhiteSpace(item.Authentication))
                            {
                                var auths = item.Authentication.FromJsonSafe<IEnumerable<KeyValuePair<string, string>>>();
                                if (auths?.Any() == true) tags.AddRange(auths.Select(p => p.Key));
                            }

                            if (!p.Tuition.HasValue || p.Tuition.Value == 0) item.Tuition = "义务教育免学费";
                            else item.Tuition = p.Tuition;

                            item.Lodging = LodgingUtil.Reason(p.Lodging, p.Sdextern);

                            if (p.EID == new Guid("e5bad31d-2de2-4b06-9512-83a7515ebffd")) Console.WriteLine(1);

                            item.Tags = tags;
                            item.Distance = eids.FirstOrDefault(x => x.EID == p.EID).Distance;
                            return item;
                        });
                        result = ResponseResult.Success(new GetSurroundExtsResponse()
                        {
                            Name = building.Name,
                            Address = building.Address,
                            Developer = building.House_Developers?.FromJsonSafe<string[]>()?.First(),
                            ImgUrls = building.New_House_Img?.FromJsonSafe<string[]>(),
                            Price = building.House_Price,
                            Property = building.House_Properties?.FromJsonSafe<string[]>()?.First(),
                            Year = building.Building_Years?.FromJsonSafe<int[]>()?.OrderBy(p => p).First() ?? 0,
                            Exts = resps.Select(p => p.Grade).Distinct().OrderBy(p => p).Select(p => new
                            {
                                Grade = p,
                                Items = resps.Where(x => x.Grade == p).OrderBy(x => x.Distance)
                            })
                        });
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 分页获取学部附近房产
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="shortNo">学部短ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="returnAllImg">是否返回全部图片</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> PageExtSurroundBuildings(Guid eid, string shortNo, int pageIndex = 2, int pageSize = 5, bool returnAllImg = false)
        {
            var result = ResponseResult.DefaultFailed();
            if ((eid == default && string.IsNullOrWhiteSpace(shortNo)) || pageIndex < 1 || pageSize < 1) return result;
            long schoolNo = 0;
            if (!string.IsNullOrWhiteSpace(shortNo))
            {
                schoolNo = UrlShortIdUtil.Base322Long(shortNo);
                if (schoolNo > 0) eid = default;
            }
            var schoolInfo = await _schoolExtensionQuery.GetSchoolExtensionDetails(eid, schoolNo);
            if (schoolInfo == default) return result;
            if (!schoolInfo.Latitude.HasValue || !schoolInfo.Longitude.HasValue) return result;
            var finds = await _schoolExtensionQuery.PageExtSurroundBuildingsAsync(schoolInfo.Longitude.Value, schoolInfo.Latitude.Value, pageIndex: pageIndex, pageSize: pageSize);
            if (finds?.Any(p => p.ID != default) == true)
            {
                var resultItems = finds.Select(p =>
                {
                    var images = string.IsNullOrWhiteSpace(p.New_House_Img) ? p.House_Img?.FromJsonSafe<string[]>() : p.New_House_Img?.FromJsonSafe<string[]>();
                    var item = new
                    {
                        p.Name,
                        Price = p.House_Price,
                        p.Address,
                        p.ID,
                        p.Distance,
                        Images = images?.Any() == true ? returnAllImg ? images : images.Take(1) : null
                    };
                    return item;
                }).OrderBy(p => p.Distance);
                if (resultItems?.Any() == true) result = ResponseResult.Success(resultItems);
            }
            return result;
        }

        /// <summary>
        /// 获取有效学部
        /// </summary>
        /// <param name="sid">学校ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetValidEIDs(Guid sid)
        {
            var result = ResponseResult.DefaultFailed();
            if (sid == default) return result;
            var finds = await _schoolExtensionQuery.ListValidEIDsAsync(sid);
            if (finds?.Any() == true) result = ResponseResult.Success(finds);
            return result;
        }
    }
}