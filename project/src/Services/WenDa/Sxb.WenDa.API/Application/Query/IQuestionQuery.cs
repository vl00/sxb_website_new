using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Enum;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Common.ResponseDto.Home;

namespace Sxb.WenDa.API.Application.Query
{
    public interface IQuestionQuery
    {
        Task<Guid> GetQuestionIdByNo(long no);

        Task<IEnumerable<QuestionDbDto>> GetQuestionDbDtos(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default);

        /// <summary>
        /// 问答item-公共头部-问题部分<br/>
        /// 可能无序
        /// </summary>
        /// <param name="ids">问题id</param>
        /// <param name="nos">问题短id</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetQaItemDtos<T>(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default, Guid userId = default)
            where T : QaItemDto, new();

        /// <summary>
        /// 填充QaQuestionListItemDto[]里面Answer字段
        /// </summary>
        Task FillQaQuestionListItemDtoByQaIds(IEnumerable<QaQuestionListItemDto> qItems, IEnumerable<(Guid Qid, Guid Aid)> qaIds, Guid userId = default);
        /// <summary>
        /// 查询QaQuestionListItemDto[]里面各个问题的最多点赞的回答
        /// </summary>
        Task FillQaQuestionListItemDtoByMaxLikeCountAnswer(IEnumerable<QaQuestionListItemDto> qItems, Guid userId = default);

        /// <summary>
        /// 根据关键字查问题，目前查前8条</summary>
        Task<GetQuestionByKeywordQueryResult> GetQuestionByKeyword(GetQuestionByKeywordQuery query);

        /// <summary>
        /// 加载问题用于编辑</summary>
        Task<LoadQuestionForSaveDto> GetQuestionByIdForSave(Guid? id);

        /// <summary>
        /// 问答详情</summary>
        Task<QuestionDetailVm> GetQuestionDetail(string questionId, string answerId, Guid userId = default);

        /// <summary>
        /// 大家热议 (不含列表)</summary>
        Task<EveryoneTalkingAboutDetailVm> GetEveryoneTalkingAboutDetail();
        /// <summary>
        /// 大家热议-列表分页</summary>
        Task<Page<QaQuestionListItemDto>> GetEveryoneTalkingAboutPageList(int pageIndex, int pageSize, Guid userId = default);

        /// <summary>
        /// 我的提问</summary>
        Task<Page<QaQuestionListItemDto>> GetMyQuestions(Guid userId, int pageIndex, int pageSize);
        /// <summary>
        /// 我的收藏-问题</summary>
        Task<Page<MyCollectQuestionListItemDto>> GetMyCollectQuestions(Guid me, int pageIndex, int pageSize);
        /// <summary>
        /// 待回答-邀请回答</summary>
        Task<Page<ToBeAnsweredQuestionListItemDto>> GetMyQuestionsToBeAnsweredWithInvitedMe(Guid me, int pageIndex, int pageSize);
        /// <summary>
        /// 待回答-推荐回答</summary>
        Task<QuestionsToBeAnsweredVm> GetMyQuestionsToBeAnswered(Guid me, int pageIndex, int pageSize);

        /// <summary>侧边栏-相关问题</summary>
        Task<IEnumerable<RelevantQuestionDto>> GetTopNRelevantQuestions(int top = 6, long? city = null, IEnumerable<long> tagIds = null, IEnumerable<long> categoryIds = null, Guid? qidNotIn = null);


        Task<List<ToBeAnsweredQuestionListItemDto>> GetWaitQuestions(Guid? userId, int top = 5);
        Task<IEnumerable<HotQuestionSchoolItemDto>> GetSchoolsAsync(ArticlePlatform platform, int top, int questionTop = 6);
        Task<IEnumerable<QuestionCategoryItemDto>> GetRandomRecommendAsync(int top);
        Task<Page<QaQuestionListItemDto>> GetQuestionPageAsync(QaListRequestDto request, Guid userId);
        Task<IEnumerable<QuestionLinkDto>> GetRecommendsAsync(ArticlePlatform platform, int city, int top);
        Task<IEnumerable<QaQuestionListItemDto>> GetQaQuestionListItemDtos(IEnumerable<Guid> ids, Guid userId);
    }
}