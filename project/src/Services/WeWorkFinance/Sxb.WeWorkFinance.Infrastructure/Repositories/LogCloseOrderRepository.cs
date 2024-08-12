using Sxb.Infrastructure.Core;
using Sxb.WeWorkFinance.Domain.AggregatesModel.LogAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Infrastructure.Repositories
{
    public class LogCloseOrderRepository : Repository<LogCloseOrder, int, UserContext>, ILogCloseOrderRepository
    {
        public LogCloseOrderRepository(UserContext context) : base(context)
        {
        }
    }
}
