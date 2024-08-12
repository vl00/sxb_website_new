using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Sxb.WenDa.Common.Consts
{
    public static partial class CacheKeys
    {
        /// <summary>
        /// 可能会被清空缓存的前缀
        /// </summary>
        const string Prefix = "wenda:";
        /// <summary>
        /// 不会被清空缓存的前缀
        /// </summary>
        const string PrefixUndeletable = "wenda_permanent:";


        ///// <summary></summary>
        //public const string Wenda_XXX = "wenda:";

        /// <summary>for delete</summary>
        public const string WendaAll = "wenda:*";

        /// <summary>问答广场-(已开通的)城市</summary>
        public const string Wenda_Citys = "wenda:citys";
        /// <summary>城市的分类</summary>
        public const string Wenda_CityCategory = "wenda:citycategory:{0}_{1}";
        /// <summary>分类的标签</summary>
        public const string Wenda_CityCategoryTags = "wenda:citycategorytags:{0}";

        /// <summary>添加问题时lock标题</summary>
        public const string Wenda_lck_addques_title = "wenda_lck:title:{0}";
        /// <summary>编辑问题时lock问题id</summary>
        public const string Wenda_lck_question = "wenda_lck:question:{0}";
        /// <summary>编辑回答时lock回答id</summary>
        public const string Wenda_lck_answer = "wenda_lck:answer:{0}";
        /// <summary>lock邀请用户回答问题</summary>
        public const string Wenda_lck_InviteUserToAnswerQuestion = "wenda_lck:invite_user_qa:q_{0}_u_{1}";
        /// <summary>发评论lock主评论id</summary>
        public const string Wenda_lck_MainCommentId = "wenda_lck:MainCommentId:{0}";
        /// <summary>专栏viewcount到db</summary>
        public const string Wenda_lck_UpSubjectViewCount_OnSyncToDB = "wenda_lck:UpSubjectViewCount_OnSyncToDB:id_{0}";

        /// <summary>
        /// 热门专栏
        /// </summary>
        public static string HomeHotSubjects = "wenda:home:hot_subjects:{0}_{1}";
        /// <summary>
        /// 大家热议
        /// </summary>
        public static string HomeHotQuestions = "wenda:home:hot_questions:{0}";
        /// <summary>
        /// 等你来回答
        /// </summary>
        public static string HomeWaitQuestions = "wenda:home:await_questions:{0}";
        /// <summary>
        /// 热门学校问答  platform_city
        /// </summary>
        public static string HomeHotSchoolQuestions = PrefixUndeletable + "home:hot_school_questions:{0}_{1}";
        /// <summary>
        /// 主站随机热门推荐
        /// </summary>
        public static string HomeRandomRecommendQuestionss = "wenda:home:hot_recommend_questions:{0}";
        /// <summary>
        /// 热门推荐  platform_city
        /// </summary>
        public static string HomeHotRecommendQuestions = "wenda:home:hot_recommend_questions:{0}_{1}";

        /// <summary>
        /// 问题列表-首页  platform_city_categoryId
        /// </summary>
        public static string Questions = "wenda:questions:{0}:{1}_{2}";


        /// <summary>问题短id to 长id</summary>
        public const string QuestionNo2Id = "wenda:qno_to_qid:no_{0}";
        /// <summary>问题</summary>
        public const string Question = "wenda:question:qid_{0}";
        /// <summary>问题的邀请用户ls</summary>
        public const string QuestionInviteUserLs = "wenda:question:qid_{0}:invite_users";
        /// <summary>问题s list</summary>
        public const string QuestionsAll = "wenda:questions:*";
        /// <summary>侧边栏-相关问题</summary>
        public const string RelevantQuestions = "wenda:questions:RelevantQuestions:city{0}_tagids{1}_cids{2}_qidnotin{3}";
        /// <summary>大家热议列表</summary>
        public const string EventOneTalkAboutLs = "wenda:questions:EventOneTalkAboutLs:p{0}s{1}";

        /// <summary>回答短id to 长id</summary>
        public const string AnswerNo2Id = "wenda:ano_to_aid:no_{0}";
        /// <summary>回答</summary>
        public const string Answer = "wenda:answer:aid_{0}";
        /// <summary>回答de评论数</summary>
        public const string AnswerCommentCount = "wenda:answer:aid_{0}:counts:comment1";
        /// <summary>回答de点赞数</summary>
        public const string AnswerLikeCount = "wenda:answer:aid_{0}:counts:likes";
        /// <summary>回答-是否我点赞过</summary>
        public const string AnswerIsLikeByMe = "wenda:answer:aid_{0}:likebyme:uid_{1}";
        /// <summary>问答详情-回答列表</summary>
        public const string QuestionAnswersPageList = "wenda:answers:qid_{0}:p{1}s{2}o{3}";
        public const string QuestionAnswersPageListAll = "wenda:answers:*";

        /// <summary>评论de点赞数</summary>
        public const string CommentLikeCount = "wenda:comment:cid_{0}:counts:likes";
        /// <summary>评论-是否我点赞过</summary>
        public const string CommentIsLikeByMe = "wenda:comment:cid_{0}:likebyme:uid_{1}";

        /// <summary>专栏短id to 长id</summary>
        public const string SubjectNo2Id = "wenda:subject_no_to_id:no_{0}";
        /// <summary>专栏</summary>
        public const string Subject = "wenda:subject:id_{0}";
        public const string SubjectQuestionsPageList = "wenda:subject:id_{0}:p{1}s{2}o{3}";
        ///// <summary>专栏-相关问题</summary>
        //public const string Subject_RelevantQuestions = "wenda:subject:id_{0}:RelevantQuestions";
        public const string SubjectViewCountIncr = "wenda_permanent:subject_viewcount_incr:id_{0}";
        /// <summary>专栏-viewcount</summary>
        public const string SubjectViewCount0 = "wenda_permanent:subject_viewcount0:id_{0}";

        /// <summary>我的问题数</summary>
        public const string MyQuestionCount = "wenda:my:uid_{0}:counts:question";
        /// <summary>我的回答数</summary>
        public const string MyAnswerCount = "wenda:my:uid_{0}:counts:answer";
        /// <summary>我的获赞数</summary>
        public const string MyGetLikeCount = "wenda:my:uid_{0}:counts:getlike";
        /// <summary>for del my</summary>
        public const string MyAll = "wenda:my:uid_{0}:*";

        /// <summary>
        /// 子站及其一级子分类
        /// </summary>
        public static string PlatformCategories = Prefix + "category:depth:1_2";
        /// <summary>
        /// 城市子站及其一级子分类 platform_city
        /// </summary>
        public static string PlatformCityCategories = Prefix + "category:depth:1_2:{0}_{1}";
        /// <summary>
        /// 子站主页分类 platform_city
        /// </summary>
        public static string PlatformHomeCategories = Prefix + "home:categories:{0}_{1}";
    }
}
