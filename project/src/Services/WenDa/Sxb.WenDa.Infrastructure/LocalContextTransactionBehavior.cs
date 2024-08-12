using Microsoft.Extensions.Logging;
using Sxb.Infrastructure.Core.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Infrastructure
{
    public class LocalContextTransactionBehavior<TRequest, TResponse> : TransactionBehavior<LocalDbContext, TRequest, TResponse>
    {
        public LocalContextTransactionBehavior(LocalDbContext dbContext, 
            ILogger<LocalContextTransactionBehavior<TRequest, TResponse>> logger) : base(dbContext, logger)
        {
        }
    }
}
