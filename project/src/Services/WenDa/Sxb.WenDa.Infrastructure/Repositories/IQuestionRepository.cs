using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Infrastructure.Repositories
{
    public interface IQuestionRepository : IRepository<Question, Guid>
    {
    }
}
