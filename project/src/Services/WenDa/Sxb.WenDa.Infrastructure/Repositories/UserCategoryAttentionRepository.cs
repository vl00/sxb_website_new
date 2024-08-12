using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;
using Sxb.WenDa.Domain.AggregateModel.UserCategoryAttentionAggregate;

namespace Sxb.WenDa.Infrastructure.Repositories
{
    public class UserCategoryAttentionRepository : Repository<UserCategoryAttention, Guid, LocalDbContext>, IUserCategoryAttentionRepository
    {
        public UserCategoryAttentionRepository(LocalDbContext context) : base(context)
        {
        }



    }
}
