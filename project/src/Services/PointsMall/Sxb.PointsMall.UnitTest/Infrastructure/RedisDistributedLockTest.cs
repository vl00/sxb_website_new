using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sxb.Framework.Cache.Redis;
using Sxb.PointsMall.API.Infrastructure.DistributedLock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.UnitTest.Infrastructure
{
    [TestClass]
    public class RedisDistributedLockTest
    {
        Mock<IEasyRedisClient> easyRedisClientMock;
        public RedisDistributedLockTest()
        {
            this.easyRedisClientMock = new Mock<IEasyRedisClient>();
        }
        [TestMethod]
        public async Task LockTakeAndWait_Test()
        {
            string key = "test", value= "1";
           var timespan =  TimeSpan.FromSeconds(30);
            easyRedisClientMock.Setup(s => s.LockTakeAsync(key, value, timespan)).ReturnsAsync(true);
            await using IDistributedLock distributedLock = new RedisDistributedLock(easyRedisClientMock.Object);
            await distributedLock.LockTakeAndWaitAsync(key, value, timespan, 30);
            Assert.IsTrue(true);
        }


        [TestMethod]
        public async Task LockTakeAndWait_Twice_Test()
        {
            string key = "test", value = "1";
            var timespan = TimeSpan.FromSeconds(30);
            easyRedisClientMock.Setup(s => s.LockTakeAsync(key, value, timespan)).ReturnsAsync(true);
            await using IDistributedLock distributedLock = new RedisDistributedLock(easyRedisClientMock.Object);
            await distributedLock.LockTakeAndWaitAsync(key, value, timespan, 10);
            easyRedisClientMock.Setup(s => s.LockTakeAsync(key, value, timespan)).ReturnsAsync(false);
            await distributedLock.LockTakeAndWaitAsync(key, value, timespan, 10);
            Assert.IsTrue(true);
        }
    }
}
