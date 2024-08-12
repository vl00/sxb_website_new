using Sxb.Framework.AspNetCoreHelper.Utils;
using Sxb.Framework.Cache.Redis;
using System;
using System.Threading.Tasks;

namespace Sxb.WenDa.API.Extensions
{
    public static class IEasyRedisClientExtensions
    {
        public static async Task<T> GetOrUpdateAsync<T>(this IEasyRedisClient easyRedisClient, string key, Func<Task<T>> func, TimeSpan updateIn, bool randomTime = true)
        {
            //不等待更新
            _ = CheckAndUpdateAsync(easyRedisClient, key, func, updateIn, randomTime).ConfigureAwait(false);

            //使用func保证有初始化数据
            return await easyRedisClient.GetOrAddAsync(key, func);
        }

        private static async Task CheckAndUpdateAsync<T>(IEasyRedisClient easyRedisClient, string key, Func<Task<T>> func, TimeSpan updateIn, bool randomTime = true)
        {
            //延时处理
            //await Task.Delay(10000);

            var timeKey = $"{key}__$time$";
            var time = await easyRedisClient.CacheRedisClient.Database.StringGetAsync(timeKey);
            if (!time.HasValue)
            {
                if (randomTime) updateIn = GetRandomTime(updateIn);

                //设置下次更新时间
                await easyRedisClient.AddAsync(timeKey, 1, updateIn);

                var data = await func.Invoke();
                //更新数据
                await easyRedisClient.AddAsync(key, data);
            }
        }

        public static TimeSpan GetRandomTime(TimeSpan time)
        {
            int maxSeconds = (int)Math.Log(time.TotalMilliseconds);
            return GetRandomTime(time, minSeconds: 0, maxSeconds);
        }

        public static TimeSpan GetRandomTime(TimeSpan time, int minSeconds = 0, int maxSeconds = 1000)
        {
            return time + TimeSpan.FromSeconds(new Random().Next(minSeconds, maxSeconds));
        }


        internal static async Task DelRedisKeys(this IEasyRedisClient easyRedisClient, IEnumerable<string> keys, int waitSec = 2)
        {
            var serviceScopeFactory = HttpContextModel.GetServiceScopeFactory();
            using var serviceScope = serviceScopeFactory.CreateScope();
            var services = serviceScope.ServiceProvider;
            await services.GetService<Common.OtherAPIClient.Org.IOrgApiService>().DelRedisKeys(keys, waitSec);
        }

    }
}
