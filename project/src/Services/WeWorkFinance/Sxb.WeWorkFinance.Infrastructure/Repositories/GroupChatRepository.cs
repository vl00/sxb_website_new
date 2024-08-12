using Microsoft.EntityFrameworkCore.Storage;
using Sxb.Infrastructure.Core;
using Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Infrastructure.Repositories
{
    public class GroupChatRepository : Repository<GroupChat, string, UserContext>, IGroupChatRepository
    {
        public GroupChatRepository(UserContext context) : base(context)
        {
        }
    }
}
