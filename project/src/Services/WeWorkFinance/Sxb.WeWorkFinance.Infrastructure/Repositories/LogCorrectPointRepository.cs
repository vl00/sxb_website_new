using Sxb.Infrastructure.Core;
using Sxb.WeWorkFinance.Domain.AggregatesModel.LogAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Infrastructure.Repositories
{
    public class LogCorrectPointRepository : Repository<LogCorrectPoint, int, UserContext>, ILogCorrectPointRepository
    {
        public LogCorrectPointRepository(UserContext context) : base(context)
        {
        }
    }
}
