using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxb.Settlement.API.Services.Aliyun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Settlement.Test
{
    [TestClass]
    public class AliyunServiceTest
    {
        public AliyunServiceTest()
        {

        }


        [TestMethod]
        public async Task BankCertificationAsync_Test()
        {
            var option = Options.Create(new AliyunOption()
            {
                AppCode = "549d9d670d85496994f3f170f7be3154"
            });
            IAliyunService aliyunService = new AliyunService(new System.Net.Http.HttpClient(), option);
            var result = await aliyunService.BankCertificationAsync(new BankCertificationRequest()
            {
                bankcard = "6212263602063084412",
                idcard = "440883199608143219",
                idcardtype = "01",
                mobile = "18218943980",
                realname = "叶剑喜",
            });
            Assert.IsTrue(result.errcode == "00000");
        }
    }
}
