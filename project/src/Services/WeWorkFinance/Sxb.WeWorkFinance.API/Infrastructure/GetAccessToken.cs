using SKIT.FlurlHttpClient.Wechat.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Infrastructure
{
    public class GetAccessToken
    {
        public static string GetToken(string corpid,string corpsecret)
        {
            var options = new WechatWorkClientOptions()
            {
                CorpId = corpid,
                AgentSecret = corpsecret
            };

            var client = new WechatWorkClient(options);
            var resp = client.ExecuteCgibinGetTokenAsync(new SKIT.FlurlHttpClient.Wechat.Work.Models.CgibinGetTokenRequest()).Result;
            if (resp.ErrorCode == 0)
            {
                return resp.AccessToken;
            }
            else
            {
                return null;
            }
        }
    }
}
