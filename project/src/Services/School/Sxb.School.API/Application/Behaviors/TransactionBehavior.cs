using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.School.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
        SchoolDataDbContext _dbcontext;
        public TransactionBehavior(SchoolDataDbContext dbcontext
            , ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = typeof(TRequest).Name;
            try
            {
                var transaction = _dbcontext.BeginTransaction();
                _logger.LogInformation("----- Begin transaction  for {CommandName} ({@Command})", typeName, request);
                response = await next();
                _dbcontext.CommitTransaction(); 
                _logger.LogInformation("----- Commit transaction  for {CommandName} ({@Command})", transaction, typeName);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}
