using Microsoft.Extensions.Logging;
using Sxb.Infrastructure.Core.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.SignActivity.Infrastructure
{
    public class OrganizationContextTransactionBehavior<TRequest, TResponse> : TransactionBehavior<OrganizationContext, TRequest, TResponse>
    {
        public OrganizationContextTransactionBehavior(OrganizationContext dbContext, ILogger<OrganizationContextTransactionBehavior<TRequest, TResponse>> logger) : base(dbContext, logger)
        {
        }
    }
}
