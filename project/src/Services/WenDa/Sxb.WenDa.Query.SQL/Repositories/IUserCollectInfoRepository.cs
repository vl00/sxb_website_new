using Sxb.WenDa.Common.Enums;

namespace Sxb.WenDa.Query.SQL.Repositories
{
    public interface IUserCollectInfoRepository
    {
        Task<IEnumerable<Guid>> GetCollectedDataIds(UserCollectType type, IEnumerable<Guid> ids, Guid userId);
    }
}