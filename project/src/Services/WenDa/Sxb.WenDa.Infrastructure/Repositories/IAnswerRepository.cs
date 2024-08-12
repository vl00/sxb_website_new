﻿using Sxb.Infrastructure.Core;
using Sxb.WenDa.Domain.AggregateModel.QuestionAggregate;

namespace Sxb.WenDa.Infrastructure.Repositories
{
    public interface IAnswerRepository : IRepository<Answer, Guid>
    {
    }
}
