using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Sxb.Static.API.Infrastruture;
using System;

namespace Sxb.Static.UnitTest
{
    [TestClass]
    public class DataViewLogRepositoryTest
    {
        [TestMethod]
        public  void TestMethod1()
        {
            IMongoClient mongoClient = new MongoClient("mongodb://172.16.0.5:27014,10.1.0.12:27014,10.1.0.17:27014/?readPreference=primary&appname=SxbStatic&ssl=false");
            DataViewLogRepository dataViewLogRepository = new DataViewLogRepository(mongoClient);
            dataViewLogRepository.AddAsync(new API.Model.DataViewLog()
            {
                Id = ObjectId.GenerateNewId(),
                CreateTime = DateTime.Now,
                DataId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                DeviceId = Guid.NewGuid(),
                DataType = API.Model.DataType.School,
                Province = 440000,
                City = 440100
            }).Wait();
            Assert.IsTrue(true);
        }
    }
}
