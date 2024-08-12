using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxb.Framework.Foundation;
using Sxb.Settlement.API.GaoDeng;
using System.Collections.Generic;
using System.Net.Http;
namespace Sxb.Settlement.Test
{
    [TestClass]
    public class GaoDengServiceTest
    {
        [TestMethod]
        public void GetSignatureTest()
        {
            using (HttpClient client = new HttpClient())
            {
                var options = Options.Create(new GaoDengOption()
                {
                    AppKey = "testappkey",
                    AppSecret = "testappsecret",
                    BaseUrl = "https://api-js-ets.wetax.com.cn/"
                });
                var logFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                });
                GaoDengService gaoDengService = new GaoDengService(options, client, logFactory.CreateLogger<GaoDengService>());
                string result = "559c4fa92e47b7f148a788300c6a345feddbd327b9fc551f100de679bbb247c5";
                var signature = gaoDengService.GetSignature("1562912285");

                Assert.IsTrue(signature.Equals(result));

            }

        }

        [TestMethod]
        public void CallBackDecodeTest()
        {
            string body = "{\"code\":0,\"msg\":\"\",\"request_id\":\"5ba93764edc144f5816ba489a4d96adb\",\"data\":\"igbAp6LLi60WbQhjtbJVkzAxx3ABhRCTlofJ7U3VOebi2xaloqi7xO5jO3ZFBGBC+8T7nnXgYT4nGCYD028J3go0WaBE6X29Nfo2go1HJvtNHgMpDNEiAiEst89w1XTKM736zyhs0uqFBLB5Ge1xUsrJmC5ZTiD3wre131TNElHq/DWteeP5yadH9jj8wvtWb4V01LwXfqmDVQw84A6+GVsLgXYL7ZzmXjur2T0jeBXCrWhXSwfZzVcwcxdjPaxMaYMNQ0TjXUU/Z+wqGG1U8s5+QsUa3Qi25Dsiyy16P3A27RB0omTfTmfh9iIVAG3n56dnhpM8xnm/eYX9Gg54IQcFVnFvKUdr4p16iiQUSayu0lUzJbftMw92b3KspNymBnhutKgNY83I6ewNg5Y9eHkGpnQ/h81B/0KM2TF9sw2WveU+WGrxNJ7W4oPWdGn2lVw71LS3OEPeQ/dtrBmFLeRYJ+hY7Oi9+70x+hW1ghsKnEn/9+QomXvdsEIfPdP/r+VSLjyzIThX733nTPbO6xEgbBKbjr+JVW12h435/Vda+dcT7YGKMWVTt5GAc0+Y7PS++ChYL3lHgFK0zKzzLLjcIUDopMRyVuwqbQJvUfs4mEwXRThjnrWbM1JaEnzu\",\"appkey\":\"112efa3797944477bad2df72073939fb\"}";
            using (HttpClient client = new HttpClient())
            {
                var options = Options.Create(new GaoDengOption()
                {
                    AppKey = "112efa3797944477bad2df72073939fb",
                    AppSecret = "56ef55128bb940fcb8623c31e1b2cd17",
                    BaseUrl = "https://api-js-ets.wetax.com.cn/",
                    AESIV = "ff465fdecc764337"
                });
                var logFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                });
                GaoDengService gaoDengService = new GaoDengService(options, client, logFactory.CreateLogger<GaoDengService>());

                var settlements =  gaoDengService.CallBackDecode<List<API.GaoDeng.Settlement>>(body);

                Assert.IsNotNull(settlements);

            }


        }



    }
}
