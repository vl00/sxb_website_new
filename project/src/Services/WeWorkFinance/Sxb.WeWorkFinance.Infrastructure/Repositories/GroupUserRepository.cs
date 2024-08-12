using Sxb.Infrastructure.Core;
using Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Infrastructure.Repositories
{
    public class GroupUserRepository : Repository<GroupUser, string, UserContext>, IGroupUserRepository
    {
        public GroupUserRepository(UserContext context) : base(context)
        {
        }
    }
}
