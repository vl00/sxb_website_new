using MediatR;
using Sxb.PointsMall.API.Application.Commands;
using Sxb.PointsMall.API.Infrastructure.DistributedLock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Behaviors
{
    public class UserExcuteOnlyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        IDistributedLockFactory  _distributedLockFactory;
        public UserExcuteOnlyBehavior(IDistributedLockFactory distributedLockFactory)
        {
            _distributedLockFactory = distributedLockFactory;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            await using var distributedLock = _distributedLockFactory.CreateRedisDistributedLock();
            var cmdName = typeof(TRequest).Name;
            bool lockTakeFlag = true;
            if (request is AddAccountPointsCommand)
            {
                //命令串行
                var cmd = request as AddAccountPointsCommand;
                lockTakeFlag =  await distributedLock.LockTakeAndWaitAsync($"{cmd.UserId}:{cmdName}", TimeSpan.FromSeconds(30), 30);
            }

            if (!lockTakeFlag)
            {
                throw new Exception("系统繁忙，请稍后重试。");
            }
            var response = await next();
            return response;
        }
    }
}
