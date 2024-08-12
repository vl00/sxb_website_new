using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enum;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.QueryDto;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public interface IQuestionRepository
    {
        Task<Common.Entity.Question> GetQuestion(Guid id = default, long no = default);
        /// <summary>根据关键字查问题，目前查前8条</summary>
        Task<List<GetQuestionByKeywordItemDto>> GetTopNQuestionByKeywords(string keyword, int count = 8);
        /// <summary>用于load编辑问题</summary>
        Task<(Common.Entity.Question Question, Guid[] Eids, long[] TagIds)> LoadQuestion(Guid id = default, long no = default);
        /// <summary></summary>
        Task<IEnumerable<QuestionDbDto>> LoadQuestions(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default);
        /// <summary>判断问题标题是否重复</summary>
        Task<bool> IsTitleExists(string title);

        /// <summary>添加问题</summary>
        Task AddQuestion(Common.Entity.Question question, List<Common.Entity.QuestionEids> quesionEids, List<Common.Entity.QuestionTag> questionTags);
        /// <summary>编辑问题</summary>
        Task EditQuestion(Common.Entity.Question question, List<Common.Entity.QuestionEids> quesionEids, List<Common.Entity.QuestionTag> questionTags);

        /// <summary>我的问题数</summary>
        Task<int> GetMyQuestionCount(Guid userId);

        /// <summary>批量获取问题s是否我收藏过的</summary>
        Task<IEnumerable<(Guid Qid, bool IsCollectedByMe)>> IsCollectedByMe(Guid userId, IEnumerable<Guid> quesionIds);

        Task<Page<Guid>> GetMyQuestionsIds(Guid userId, int pageIndex, int pageSize);
        Task<Page<Guid>> GetUserCollectQuestions(Guid userId, int pageIndex, int pageSize);

        /// <summary>我的待回答-邀请我来回答</summary>
        Task<Page<Guid>> GetUserQuestionsToBeAnsweredByInvited(Guid userId, int pageIndex, int pageSize);
        /// <summary>我的待回答-推荐回答</summary>
        Task<Page<Guid>> GetUserQuestionsToBeAnswered(Guid userId, IEnumerable<long> categoryIds, IEnumerable<long> platforms, int pageIndex, int pageSize);

        /// <summary>侧边栏-相关问题</summary>
        Task<IEnumerable<RelevantQuestionDto>> GetTopNRelevantQuestions(int top = 6, long? city = null, IEnumerable<long> tagIds = null, IEnumerable<long> categoryIds = null, IEnumerable<Guid> qidsNotIn = null);

        /// <summary>大家热议</summary>
        Task<Page<Guid>> GetEveryoneTalkingAboutPageList(int pageIndex, int pageSize);

        Task<IEnumerable<QuestionLinkDto>> GetLinkListAsync(QuestionOrderBy orderBy, ArticlePlatform? platform, long? city, Guid? subjectId, int top);
        Task<IEnumerable<QuestionAndAnswerLinkDto>> GetQuestionAndAnswerLinkListAsync(QuestionOrderBy orderBy, ArticlePlatform? platform, long? city, Guid? subjectId, int top);
        Task<IEnumerable<SchoolQuestionLinkDto>> GetSchoolQuestionLinksAsync(IEnumerable<Guid> extIds, QuestionOrderBy orderBy, int top);
        Task<IEnumerable<QuestionLinkDto>> GetHotsAsync(int top);
        Task<IEnumerable<WaitQuestionItemDto>> GetWaitsAsync(IEnumerable<long> categoryIds, Guid exclundUserId, int top);
        Task<IEnumerable<QuestionCategoryItemDto>> GetRandomRecommendAsync(int top);
        Task<Page<Question>> GetQuestionPageAsync(ArticlePlatform platform, int city, IEnumerable<long> categoryIds, int pageIndex, int pageSize);
        Task<IEnumerable<Question>> GetQuestionsAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<Question>> GetQuestionsAsync(ArticlePlatform platform, int city, IEnumerable<long> categoryIds, int pageIndex, int pageSize);
        Task<IEnumerable<NotifyUserQueryDto>> GetNewAnswerUserAsync(DateTime startTime, DateTime endTime, int pageIndex, int pageSize);
        Task BatchAddQuestions(List<Question> questions, List<QuestionAnswer> answers, List<QuestionTag> questionTags);
    }
}
