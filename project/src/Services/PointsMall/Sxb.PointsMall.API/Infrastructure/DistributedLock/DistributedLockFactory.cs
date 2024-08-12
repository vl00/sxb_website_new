using Sxb.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Infrastructure.DistributedLock
{
    public class DistributedLockFactory: IDistributedLockFactory
    {
        IEasyRedisClient easyRedisClient;
        int _maxWaitSeconds;

        public DistributedLockFactory(IEasyRedisClient easyRedisClient,int maxWaitSeconds = 60)
        {
            this.easyRedisClient = easyRedisClient;
            _maxWaitSeconds = maxWaitSeconds;
        }

        public IDistributedLock CreateRedisDistributedLock()
        {
            return new RedisDistributedLock(this.easyRedisClient, _maxWaitSeconds);
        }
    }
}
