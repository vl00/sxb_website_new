using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.API.Application.Query
{
    public interface IAnswerQuery
    {
        /// <summary>
        /// 加载回答用于编辑</summary>
        Task<LoadAnswerForSaveDto> GetAnswerByIdForSave(Guid id);

        Task<Page<QaAnswerItemDto>> GetAnswersPageList(Guid questionId, int pageIndex, int pageSize, AnswersListOrderByEnum orderby, Guid userId = default);

        /// <summary>
        /// 问答item-回答部分<br/>
        /// 可能无序
        /// </summary>
        /// <param name="ids">回答id</param>
        /// <param name="nos">回答no</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<QaAnswerItemDto>> GetQaAnswerItemDtos(IEnumerable<Guid> ids = default, IEnumerable<long> nos = default, Guid userId = default);
        /// <summary>
        /// 回答-评论数</summary>
        Task<IEnumerable<(Guid, int)>> GetAnswersReplyCounts(IEnumerable<Guid> ids);
        /// <summary>
        /// 回答-点赞数+是否我点赞</summary>
        Task<IEnumerable<LikeCountDto>> GetAnswersLikeCounts(IEnumerable<Guid> ids, Guid userId = default);

        /// <summary>
        /// 加载邀请人列表
        /// </summary>
        Task<GetQuestionInviteUserLsQueryResult> GetQuesInviteUserList(GetQuestionInviteUserLsQuery query);

        /// <summary>
        /// 我的回答 <br/>
        /// 同一问题，如果用户回答了多次，是显示多条还是1条(最近 or 点赞最多)？显示1条最近的
        /// </summary>
        Task<Page<QaQuestionListItemDto>> GetMyAnswers(Guid userId, int pageIndex, int pageSize);
    }
}