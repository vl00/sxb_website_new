using Sxb.Infrastructure.Core;
using Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Infrastructure.Repositories
{
    public class GroupMemberRepository : Repository<GroupMember, Guid, UserContext>, IGroupMemberRepository
    {
        public GroupMemberRepository(UserContext context) : base(context)
        {
        }
    }
}
