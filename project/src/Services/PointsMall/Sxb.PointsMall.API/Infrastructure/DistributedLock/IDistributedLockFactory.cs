using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Infrastructure.DistributedLock
{
    public interface IDistributedLockFactory
    {
        IDistributedLock CreateRedisDistributedLock();

    }
}
