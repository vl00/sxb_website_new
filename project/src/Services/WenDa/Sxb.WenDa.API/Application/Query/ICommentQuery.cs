using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.Repositories;

namespace Sxb.WenDa.API.Application.Query
{
    public interface ICommentQuery
    {
        Task<GetCommentsPageListQryResDto> GetCommentsPageList(GetCommentsPageListQuery query);

        Task<IEnumerable<LikeCountDto>> GetCommentsLikeCounts(IEnumerable<Guid> ids, Guid userId = default);
    }
}