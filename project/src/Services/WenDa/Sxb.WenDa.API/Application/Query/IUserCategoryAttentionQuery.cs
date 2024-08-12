using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.Repositories;

namespace Sxb.WenDa.API.Application.Query
{
    public interface IUserCategoryAttentionQuery
    {
        Task<IEnumerable<AttentionCategoryDto>> GetUserCategories(Guid userId);
    }
}