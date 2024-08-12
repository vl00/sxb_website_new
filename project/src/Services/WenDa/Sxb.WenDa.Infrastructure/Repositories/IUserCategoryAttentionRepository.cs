using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.UserCategoryAttentionAggregate;

namespace Sxb.WenDa.Infrastructure.Repositories
{
    public interface IUserCategoryAttentionRepository : IRepository<UserCategoryAttention, Guid>
    {
    }
}