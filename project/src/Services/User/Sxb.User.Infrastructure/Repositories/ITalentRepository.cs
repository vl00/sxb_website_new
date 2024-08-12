using Sxb.Infrastructure.Core;
using Sxb.User.Domain.AggregatesModel.TalentAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.User.Infrastructure.Repositories
{
    public interface ITalentRepository : IRepository<Talent, Guid>
    {
    }
}
