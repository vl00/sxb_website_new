using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public interface IAnswerRepository
    {
        /// <summary>加载回答用于编辑</summary>
        Task<QuestionAnswer> GetAnswer(Guid id = default, long no = default);

        Task<IEnumerable<QuestionAnswer>> LoadQaAnswers(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default);

        Task<bool> DeleteAnswer(QuestionAnswer answer, Question question = null);
        Task<bool> EditAnswer(QuestionAnswer answer);
        Task AddAnswer(QuestionAnswer answer, Question question = null, Invitation invitation = null);

        IAsyncEnumerable<(Guid UserId, bool IsInvited)> GetUsersIsInvitedToQuesion(Guid qid, IEnumerable<Guid> userIds);
        Task<Invitation> GetInvitation(Guid qid, Guid toUserId);
        Task AddInvitation(Invitation invitation);

        /// <summary>我的回答数</summary>
        Task<int> GetMyAnswerCount(Guid userId);
        /// <summary>我的回答的获赞数</summary>
        Task<int> GetMyAnswerGetLikeCount(Guid userId);

        /// <summary>回答-评论数</summary>
        Task<IEnumerable<(Guid, int)>> GetAnswersReplyCounts(IEnumerable<Guid> ids);
        /// <summary>回答-点赞数</summary>
        Task<IEnumerable<(Guid, int)>> GetAnswersLikeCounts(IEnumerable<Guid> ids);
        /// <summary>回答的内容</summary>
        Task<IEnumerable<(Guid Aid, string Content)>> GetQaAnswersContent(IEnumerable<Guid> ids);


        Task<Page<Guid>> GetAnswersPageList(Guid questionId, int pageIndex, int pageSize, AnswersListOrderByEnum orderby);

        /// <summary>我的回答列表</summary>
        Task<Page<(Guid Qid, Guid Aid)>> GetIdsByMyAnswers(Guid userId, int pageIndex, int pageSize);

        /// <summary>
        /// 根据问题s查 问题中最多点赞的回答s
        /// </summary>
        Task<IEnumerable<(Guid Qid, Guid Aid)>> GetAnswersIdsWithMaxLikeCountInQuestion(IEnumerable<Guid> qids);

        /// <summary>检查问题s是否我回答过</summary>
        Task<IEnumerable<(Guid Qid, bool HasMyAnswers)>> GetQuestionsIfHasMyAnswers(Guid userId, IEnumerable<Guid> qids);
    }
}
