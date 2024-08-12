using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;

namespace Sxb.WenDa.Infrastructure.Repositories
{
    public class QuestionRepository : Repository<Question, Guid, LocalDbContext>, IQuestionRepository
    {
        public QuestionRepository(LocalDbContext context) : base(context)
        {
        }
    }
}
