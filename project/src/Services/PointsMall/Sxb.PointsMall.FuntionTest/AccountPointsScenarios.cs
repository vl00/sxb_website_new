using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PointsMall.FuntionTest
{
    [TestClass]
    public class AccountPointsScenarios : AccountPointsScenarioBase
    {
        [TestMethod]
        public async Task AccessHome()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateIdempotentClient()
                    .GetAsync(Get.Home);

                response.EnsureSuccessStatusCode();
            }
        }



        [TestMethod]
        public async Task PointsDetail()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateIdempotentClient()
                    .GetAsync(Get.PointsDetail);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAs<ResponseResult>();
                Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
            }
        }



        [TestMethod]
        public async Task DaySignIn()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateIdempotentClient()
                    .GetAsync(Get.DaySignIn);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAs<ResponseResult>();
                Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
            }
        }




        [TestMethod]
        public async Task FreezePoints()
        {
            using (var server = CreateServer())
            {
                var cmd = new API.Application.Commands.FreezePointsCommand() { FreezePoints = 10, OriginId = "xxxx", OriginType = Domain.AggregatesModel.PointsAggregate.AccountPointsOriginType.Orders, Remark = "xxx,零元购", UserId = Guid.Parse("54CDA0AA-3270-42D6-9E74-41A4B1591868") };
                var content = JsonContent.Create(cmd);
                var response = await server.CreateIdempotentClient()
                    .PostAsync(Post.FreezePoints, content);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAs<ResponseResult>();
                Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
            }
        }

        [TestMethod]
        public async Task AddFreezePoints()
        {
            using (var server = CreateServer())
            {
                var cmd = new API.Application.Commands.AddAccountFreezePointsCommand() { FreezePoints = 10, OriginId = "xxxx", OriginType = Domain.AggregatesModel.PointsAggregate.AccountPointsOriginType.Orders, Remark = "xxx,零元购", UserId = Guid.Parse("54CDA0AA-3270-42D6-9E74-41A4B1591868") };
                var content = JsonContent.Create(cmd);
                var response = await server.CreateIdempotentClient()
                    .PostAsync(Post.AddFreezePoints, content);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAs<ResponseResult>();
                Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
            }
        }



        [TestMethod]
        public async Task DeFreezePoints()
        {
            using (var server = CreateServer())
            {
                var cmd = new API.Application.Commands.DeFreezePointsCommand()
                {
                    FreezeId = await GetFreezeId(server)
                    ,
                    UserId = Guid.Parse("54CDA0AA-3270-42D6-9E74-41A4B1591868")
                };
                var content = JsonContent.Create(cmd);
                var response = await server.CreateIdempotentClient().PostAsync(Post.DeFreezePoints, content);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAs<ResponseResult>();
                Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
            }
        }


        [TestMethod]
        public async Task DeFreezePointsAfterAddFreezePoints()
        {
            using (var server = CreateServer())
            {
                var cmd = new API.Application.Commands.DeFreezePointsCommand()
                {
                    FreezeId = await GetAddFreezeId(server)
                    ,
                    UserId = Guid.Parse("54CDA0AA-3270-42D6-9E74-41A4B1591868")

                };
                var content = JsonContent.Create(cmd);
                var response = await server.CreateIdempotentClient().PostAsync(Post.DeFreezePoints, content);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAs<ResponseResult>();
                Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
            }
        }

        [TestMethod]
        public async Task DeductFreezePoints()
        {
            using (var server = CreateServer())
            {
                var cmd = new API.Application.Commands.DeductFreezePointsCommand()
                {
                    FreezeId = await GetFreezeId(server)
             ,
                    UserId = Guid.Parse("54CDA0AA-3270-42D6-9E74-41A4B1591868")
                    ,
                    OriginType = Domain.AggregatesModel.PointsAggregate.AccountPointsOriginType.OrderLoseEfficacy

                };
                var content = JsonContent.Create(cmd);
                var response = await server.CreateIdempotentClient().PostAsync(Post.DeductFreezePoints, content);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAs<ResponseResult>();
                Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
            }
        }



        [TestMethod]
        public async Task DeductFreezePointsAfterAddFreezePoints()
        {
            using (var server = CreateServer())
            {
                var cmd = new API.Application.Commands.DeductFreezePointsCommand()
                {
                    FreezeId = await GetAddFreezeId(server)
             ,
                    UserId = Guid.Parse("54CDA0AA-3270-42D6-9E74-41A4B1591868")
                     ,
                    OriginType = Domain.AggregatesModel.PointsAggregate.AccountPointsOriginType.OrderLoseEfficacy
                };
                var content = JsonContent.Create(cmd);
                var response = await server.CreateIdempotentClient().PostAsync(Post.DeductFreezePoints, content);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAs<ResponseResult>();
                Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
            }
        }
        [TestMethod]
        public async Task AddAccountPoints()
        {
            using (var server = CreateServer())
            {
                var cmd = new API.Application.Commands.AddAccountPointsCommand()
                {
                    UserId = Guid.Parse("54CDA0AA-3270-42D6-9E74-41A4B1591868"),
                    OriginId = "ooooooxxxxxooooooo",
                    Remark = "【黑人抬棺】下单成功。",
                    OriginType = Domain.AggregatesModel.PointsAggregate.AccountPointsOriginType.Orders,
                    Points = 100
                };
                var content = JsonContent.Create(cmd);
                var response = await server.CreateIdempotentClient().PostAsync(Post.AddAccountPoints, content);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAs<ResponseResult>();
                Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
            }
        }



        [TestMethod]
        public async Task NotifyPointExpire()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateIdempotentClient().GetAsync(Get.NotifyPointExpire);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAs<ResponseResult>();
                Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
            }
        }


        async Task<Guid> GetFreezeId(TestServer server)
        {

            var cmd = new API.Application.Commands.FreezePointsCommand() { FreezePoints = 10, OriginId = Guid.NewGuid().ToString(), OriginType = Domain.AggregatesModel.PointsAggregate.AccountPointsOriginType.Orders, Remark = "xxx,零元购", UserId = Guid.Parse("54CDA0AA-3270-42D6-9E74-41A4B1591868") };
            var content = JsonContent.Create(cmd);
            var freezeIdResponse = await server.CreateIdempotentClient().PostAsync(Post.FreezePoints, content);
            var freezeId = Guid.Parse(JObject.Parse(await freezeIdResponse.Content.ReadAsStringAsync())["data"]["freezeId"].Value<string>());
            return freezeId;
        }




        async Task<Guid> GetAddFreezeId(TestServer server)
        {

            var cmd = new API.Application.Commands.AddAccountFreezePointsCommand() { FreezePoints = 10, OriginId = Guid.NewGuid().ToString(), OriginType = Domain.AggregatesModel.PointsAggregate.AccountPointsOriginType.Orders, Remark = "xxx,零元购", UserId = Guid.Parse("54CDA0AA-3270-42D6-9E74-41A4B1591868") };
            var content = JsonContent.Create(cmd);
            var freezeIdResponse = await server.CreateIdempotentClient().PostAsync(Post.AddFreezePoints, content);
            var freezeId = Guid.Parse(JObject.Parse(await freezeIdResponse.Content.ReadAsStringAsync())["data"]["freezeId"].Value<string>());
            return freezeId;

        }


    }
}
