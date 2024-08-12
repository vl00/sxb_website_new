using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.LikeAggregate;

namespace Sxb.WenDa.Infrastructure.Repositories
{
    public class UserLikeInfoRepository : Repository<UserLikeInfo, Guid, LocalDbContext>, IUserLikeInfoRepository
    {
        public UserLikeInfoRepository(LocalDbContext context) : base(context)
        {
        }
    }
}
