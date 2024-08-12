using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Infrastructure.DistributedLock
{
    public interface IDistributedLock : IAsyncDisposable
    {

        Task<bool> LockTakeAsync(string key, string value, TimeSpan expire);
        Task<bool> LockReleaseAsync(string key, string value);

        Task<bool> LockTakeAndWaitAsync(string key, string value, TimeSpan expire, int waitSeconds = 30);
        Task<bool> LockTakeAndWaitAsync(string key, TimeSpan expire, int waitSeconds = 30);
        Task LockTakeAndWaitWillThrowExceptionWhenOverTimeAsync(string key, string value, TimeSpan expire, int waitSeconds = 30);

        Task LockTakeAndWaitWillThrowExceptionWhenOverTimeAsync(string key, TimeSpan expire, int waitSeconds = 30);

    }
}
