
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.School.API;
using Sxb.School.API.Application.Models;
using Sxb.School.API.Models;
using Sxb.School.API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.FuntionTest
{
    [TestClass]
    public class ViewSchoolScenarios : SchoolScenariosBase
    {
        [TestMethod]
        public async Task GetQRCode_Test()
        {
            var client = base.CreateIdempotentClient();
            var response = await client
                .GetAsync(Get.GetQRCode(Guid.NewGuid()));
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAs<ResponseResult>();
            Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
        }

        [TestMethod]
        public async Task GetQRCodeState_Test()
        {
            var client = base.CreateIdempotentClient();
            var response1 = await client
                .GetAsync(Get.GetQRCode(Guid.NewGuid()));
            response1.EnsureSuccessStatusCode();
            var orderId = Guid.Parse(JObject.Parse(await response1.Content.ReadAsStringAsync())["data"]["orderId"].Value<string>());
            var response = await client.GetAsync(Get.GetQRCodeState(orderId));
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAs<ResponseResult>();
            Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
        }

        [TestMethod]
        public async Task GetQRCodeState_WhenPaySucces_Test()
        {
            var client = base.CreateIdempotentClient();
            //创建订单
            var response1 = await client.GetAsync(Get.GetQRCode(Guid.NewGuid()));
            response1.EnsureSuccessStatusCode();
            var orderId = Guid.Parse(JObject.Parse(await response1.Content.ReadAsStringAsync())["data"]["orderId"].Value<string>());
            //支付成功
            PayCallBackData callBackData = new PayCallBackData()
            {
                AddTime = DateTime.Now,
                OrderId = orderId,
                OrderNo = "xxxxxxx1",
                PayStatus = PayCallBackData.PayStatusEnum.Success
            };
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(callBackData);
            HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
            PayRequestHelper.SetSignHeader(content, Configs.PayKey, body, Configs.PaySystem);
            var response = await client.PostAsync(Post.RCVPAYCB_VS, content);
            response.EnsureSuccessStatusCode();
            //查状态
            var response2 = await client.GetAsync(Get.GetQRCodeState(orderId));
            response2.EnsureSuccessStatusCode();
            var result2 = await response.Content.ReadAs<ResponseResult>();
            Assert.IsTrue(result2.status != ResponseCode.Error, result2.Msg);
        }



        [TestMethod]
        public async Task MockRCVWPCB_VS_Test()
        {
            var client = base.CreateIdempotentClient();
            var response1 = await client
                .GetAsync(Get.GetQRCode(Guid.NewGuid()));
            response1.EnsureSuccessStatusCode();
            var orderId = Guid.Parse(JObject.Parse(await response1.Content.ReadAsStringAsync())["data"]["orderId"].Value<string>());
            ViewSchoolScanCallBackAttachData attachData = new ViewSchoolScanCallBackAttachData()
            {
                OrderId = orderId
            };

            WPScanCallBackData callBackData = new WPScanCallBackData()
            {
                AppId = "xxxx",
                IsSubscribe = true,
                OpenId = "oieSE6I0kXEmqRa4iwYS0JCCclAw",
                UnionId = "xxxxxx",
                SubscribeTime = 1232132132,
                Attach = Newtonsoft.Json.JsonConvert.SerializeObject(attachData),
                UserId = Guid.Parse("54CDA0AA-3270-42D6-9E74-41A4B1591867")
            };
            HttpContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(callBackData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Post.RCVWPCB_VS("xxxxxxx"), content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAs<ResponseResult>();
            Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
        }




        [TestMethod]
        public async Task MockRCVWPCB_VS_Free_Test()
        {
            var client = base.CreateIdempotentClient();
            var response1 = await client
                .GetAsync(Get.GetQRCode(Guid.Parse("0DF09F68-81AD-4069-A917-0001571B21DB")));
            response1.EnsureSuccessStatusCode();
            var orderId = Guid.Parse(JObject.Parse(await response1.Content.ReadAsStringAsync())["data"]["orderId"].Value<string>());
            ViewSchoolScanCallBackAttachData attachData = new ViewSchoolScanCallBackAttachData()
            {
                OrderId = orderId, Mode = 1
            };

            WPScanCallBackData callBackData = new WPScanCallBackData()
            {
                AppId = "xxxx",
                IsSubscribe = true,
                OpenId = "oieSE6I0kXEmqRa4iwYS0JCCclAw",
                UnionId = "xxxxxx",
                SubscribeTime = 1232132132,
                Attach = Newtonsoft.Json.JsonConvert.SerializeObject(attachData),
                UserId = Guid.Parse("54CDA0AA-3270-42D6-9E74-41A4B1591867")
            };
            HttpContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(callBackData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Post.RCVWPCB_VS("xxxxxxx"), content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAs<ResponseResult>();
            Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
        }




        [TestMethod]
        public async Task MockRCVPAYCB_VS_PaySuccess_Test()
        {
            var client = base.CreateIdempotentClient();
            var response1 = await client
                .GetAsync(Get.GetQRCode(Guid.NewGuid()));
            response1.EnsureSuccessStatusCode();
            var orderId = Guid.Parse(JObject.Parse(await response1.Content.ReadAsStringAsync())["data"]["orderId"].Value<string>());
            PayCallBackData callBackData = new PayCallBackData()
            {
                AddTime = DateTime.Now,
                OrderId = orderId,
                OrderNo = "xxxxxxx1",
                PayStatus = PayCallBackData.PayStatusEnum.Success
            };
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(callBackData);
            HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
            PayRequestHelper.SetSignHeader(content, Configs.PayKey, body, Configs.PaySystem);
            var response = await client.PostAsync(Post.RCVPAYCB_VS, content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAs<ResponseResult>();
            Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
        }


        [TestMethod]
        public async Task MockRCVPAYCB_VS_PayFail_Test()
        {
            var client = base.CreateIdempotentClient();
            var response1 = await client
                .GetAsync(Get.GetQRCode(Guid.NewGuid()));
            response1.EnsureSuccessStatusCode();
            var orderId = Guid.Parse(JObject.Parse(await response1.Content.ReadAsStringAsync())["data"]["orderId"].Value<string>());
            PayCallBackData callBackData = new PayCallBackData()
            {
                AddTime = DateTime.Now,
                OrderId = orderId,
                OrderNo = "xxxxxxx1",
                PayStatus = PayCallBackData.PayStatusEnum.Fail
            };
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(callBackData);
            HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
            PayRequestHelper.SetSignHeader(content, Configs.PayKey, body, Configs.PaySystem);
            var response = await client.PostAsync(Post.RCVPAYCB_VS, content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAs<ResponseResult>();
            Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
        }

        [TestMethod]
        public async Task MockRCVWXWORKCB_VS_Test()
        {
            var client = base.CreateIdempotentClient();

            WxWorkAddCustomerCallBackData callBackData = new WxWorkAddCustomerCallBackData()
            {
             OrderId = Guid.Parse("62FFE613-35AA-413D-8FCB-4121ED3428B6")
            };
            HttpContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(callBackData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Post.RCVWXWORKCB_VS, content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAs<ResponseResult>();
            Assert.IsTrue(result.status != ResponseCode.Error, result.Msg);
        }

        [TestMethod]
        public async Task DecimalTostring()
        {
            decimal a = 1.01M;
            decimal b = 1.293M;
            string a1 = a.ToString("0.##");
            string b1 = b.ToString("0.##");
            Assert.IsTrue(true);
        }

    }
}
