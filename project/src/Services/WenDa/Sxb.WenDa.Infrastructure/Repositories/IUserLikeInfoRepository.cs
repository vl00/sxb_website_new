using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.LikeAggregate;

namespace Sxb.WenDa.Infrastructure.Repositories
{
    public interface IUserLikeInfoRepository : IRepository<UserLikeInfo, Guid>
    {
    }
}