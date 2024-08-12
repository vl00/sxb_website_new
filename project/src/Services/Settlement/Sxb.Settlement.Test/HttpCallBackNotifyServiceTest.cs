using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxb.Settlement.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Settlement.Test
{
    [TestClass]
    public class HttpCallBackNotifyServiceTest
    {
        [TestMethod]
        public async Task NotifySettlementStatus_Test()
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<HttpCallBackNotifyService>();
            HttpClient client = new HttpClient();
            HttpCallBackNotifyService httpCallBackNotifyService = new HttpCallBackNotifyService(client, logger);
            await httpCallBackNotifyService.NotifySettlementStatus("https://www.sxkid.com", new API.SettlementStatusMessage()
            {
                OrderNum = "xxxx.sxxxs.ss",
                Status =  API.Model.SettlementStatus.Success,
            });
            Assert.IsTrue(true);
        }


        [TestMethod]
        public async Task NotifySettlementRefundSuccess_Test()
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<HttpCallBackNotifyService>();
            HttpClient client = new HttpClient();
            HttpCallBackNotifyService httpCallBackNotifyService = new HttpCallBackNotifyService(client, logger);
            await httpCallBackNotifyService.NotifySettlementRefundSuccess("https://www.sxkid.com", new API.SettlementRefundSuccessMessage()
            {
                OrderNum = "xxxxx.ssxx"
            });
            Assert.IsTrue(true);
        }
    }
}
