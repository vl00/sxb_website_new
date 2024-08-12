using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;

namespace Sxb.WenDa.Infrastructure.Repositories
{
    public class AnswerRepository : Repository<Answer, Guid, LocalDbContext>, IAnswerRepository
    {
        public AnswerRepository(LocalDbContext context) : base(context)
        {
        }
    }
}
