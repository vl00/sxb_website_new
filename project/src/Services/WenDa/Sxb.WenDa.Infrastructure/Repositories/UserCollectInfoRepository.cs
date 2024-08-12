using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;

namespace Sxb.WenDa.Infrastructure.Repositories
{
    public class UserCollectInfoRepository : Repository<UserCollectInfo, Guid, LocalDbContext>, IUserCollectInfoRepository
    {
        public UserCollectInfoRepository(LocalDbContext context) : base(context)
        {
        }
    }
}
