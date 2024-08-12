using Sxb.Infrastructure.Core;
using Sxb.User.Domain.AggregatesModel.TalentAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.User.Infrastructure.Repositories
{
    public class TalentRepository : Repository<Talent, Guid, UserContext>, ITalentRepository
    {
        public TalentRepository(UserContext context) : base(context)
        {
        }
    }
}
