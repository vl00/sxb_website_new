using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.ResponseDto;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public interface IUserRepository
    {
        Task<RealUser> GetRealUser(Guid userId);

        Task SaveRealUser(RealUser rUser);

        /// <summary>是否我点赞过</summary>
        Task<IEnumerable<(Guid, bool)>> GetsIsLikeByMe(IEnumerable<Guid> ids, Guid userId, UserLikeType likeType);

    }
}
