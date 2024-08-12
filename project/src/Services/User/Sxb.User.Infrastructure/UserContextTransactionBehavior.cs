using Microsoft.Extensions.Logging;
using Sxb.Infrastructure.Core.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.User.Infrastructure
{
    public class UserContextTransactionBehavior<TRequest, TResponse> : TransactionBehavior<UserContext, TRequest, TResponse>
    {
        public UserContextTransactionBehavior(UserContext dbContext, ILogger<UserContextTransactionBehavior<TRequest, TResponse>> logger) : base(dbContext, logger)
        {
        }
    }
}
