using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;
using Sxb.WenDa.Domain.AggregateModel.SubjectAggregate;

namespace Sxb.WenDa.Infrastructure.Repositories
{
    public class SubjectRepository : Repository<Subject, Guid, LocalDbContext>, ISubjectRepository
    {
        public SubjectRepository(LocalDbContext context) : base(context)
        {
        }
    }
}
