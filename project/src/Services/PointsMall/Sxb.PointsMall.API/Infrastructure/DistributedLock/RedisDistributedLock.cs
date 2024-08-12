using Sxb.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Infrastructure.DistributedLock
{
    public class RedisDistributedLock : IDistributedLock
    {
        private int _waitMaxSeconds;
        IEasyRedisClient _easyRedisClient;
        Dictionary<string, string> kvs = new Dictionary<string, string>();


        public RedisDistributedLock(IEasyRedisClient easyRedisClient, int waitMaxSeconds = 60)
        {
            _easyRedisClient = easyRedisClient;
            _waitMaxSeconds = waitMaxSeconds;
        }


        public async Task<bool> LockReleaseAsync(string key, string value)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    bool flag = await _easyRedisClient.LockReleaseAsync(key, value);
                    if (flag) return true;
                }
                catch { }
            }
            return false;

        }

        public async Task<bool> LockTakeAndWaitAsync(string key, string value, TimeSpan expire, int waitSeconds = 30)
        {

            int secondCounter = 0;
            if (waitSeconds > _waitMaxSeconds) throw new ArgumentException($"waitSeconds max {_waitMaxSeconds}s.");
            try
            {
                while (true)
                {
                    var flag = await _easyRedisClient.LockTakeAsync(key, value, expire);
                    if (flag)
                    {
                        kvs[key] = value;
                        return true;
                    }
                    if (secondCounter >= waitSeconds) return false;
                    await Task.Delay(1000);
                    secondCounter++;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LockTakeAsync(string key, string value, TimeSpan expire)
        {
            try
            {
                return await _easyRedisClient.LockTakeAsync(key, value, expire);
            }
            catch
            {
                return false;
            }
        }



        public async ValueTask DisposeAsync()
        {
            foreach (var kv in kvs)
            {
                await this.LockReleaseAsync(kv.Key, kv.Value);
            }
            kvs = null;
            GC.SuppressFinalize(this);
        }

        public async Task<bool> LockTakeAndWaitAsync(string key, TimeSpan expire, int waitSeconds = 30)
        {
            return await this.LockTakeAndWaitAsync(key, key, expire,waitSeconds);
        }

        public async Task LockTakeAndWaitWillThrowExceptionWhenOverTimeAsync(string key, string value, TimeSpan expire, int waitSeconds = 30)
        {
            int secondCounter = 0;
            if (waitSeconds > _waitMaxSeconds) throw new ArgumentException($"waitSeconds max {_waitMaxSeconds}s.");
            while (true)
            {
                var flag = await _easyRedisClient.LockTakeAsync(key, value, expire);
                if (flag)
                {
                    kvs[key] = value;
                    return;
                }
                if (secondCounter >= waitSeconds) throw new Exception("锁等待超时。");
                await Task.Delay(1000);
                secondCounter++;
            }
        }


        public async Task LockTakeAndWaitWillThrowExceptionWhenOverTimeAsync(string key, TimeSpan expire, int waitSeconds = 30)
        {
             await this.LockTakeAndWaitWillThrowExceptionWhenOverTimeAsync(key, key, expire, waitSeconds);
        }
    }
}
