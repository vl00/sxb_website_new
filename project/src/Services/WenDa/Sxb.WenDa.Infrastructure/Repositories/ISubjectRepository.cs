using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.SubjectAggregate;

namespace Sxb.WenDa.Infrastructure.Repositories
{
    public interface ISubjectRepository : IRepository<Subject, Guid>
    {
    }
}
