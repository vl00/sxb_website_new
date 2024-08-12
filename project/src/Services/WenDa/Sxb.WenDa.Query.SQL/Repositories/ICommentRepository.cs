using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.QueryDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public interface ICommentRepository
    {
        Task<QaComment> GetComment(Guid id);

        Task AddComment(QaComment comment, QaComment commentFrom = null, QuestionAnswer answer = null);

        /// <summary>我的评论的获赞数</summary>
        Task<int> GetMyCommentLikeCount(Guid userId);

        Task<Page<CommentItemDto>> GetCommentsPageList(GetCommentsPageListQuery query);
        Task<IEnumerable<(Guid, CommentItemDto[])>> GetTop2ChildrenCommentsByMainCommentIds(IEnumerable<Guid> mainCommentIds);

        /// <summary>评论的点赞数</summary>
        Task<IEnumerable<(Guid, int)>> GetCommentsLikeCounts(IEnumerable<Guid> ids);
        Task<IEnumerable<NotifyUserQueryDto>> GetCommentFromUserAsync(DateTime startTime, DateTime endTime, int pageIndex, int pageSize);
    }
}
