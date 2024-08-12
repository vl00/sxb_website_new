using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;

namespace Sxb.WenDa.Infrastructure.Repositories
{
    public class CommentRepository : Repository<Comment, Guid, LocalDbContext>, ICommentRepository
    {
        public CommentRepository(LocalDbContext context) : base(context)
        {
        }
    }
}
