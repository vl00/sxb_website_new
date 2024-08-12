using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.Settlement.API.Services.Aliyun
{
    public class AliyunService: IAliyunService
    {
        AliyunOption AliyunOption;
        HttpClient _client;

        public AliyunService(HttpClient client,IOptions<AliyunOption> options)
        {
            _client = client;
            AliyunOption = options.Value;
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "APPCODE " + AliyunOption.AppCode);

        }

        public async Task<AliyunResponse<BankCertificationResult>> BankCertificationAsync(BankCertificationRequest request)
        {
            string url = "https://b234bzxsv1.market.alicloudapi.com/bank/4bzxsv1";
            List<KeyValuePair<string, string>> keyValuePairs =new List<KeyValuePair<string, string>>();
            keyValuePairs.Add(new KeyValuePair<string, string>(nameof(request.bankcard), request.bankcard));
            keyValuePairs.Add(new KeyValuePair<string, string>(nameof(request.customername), request.customername));
            keyValuePairs.Add(new KeyValuePair<string, string>(nameof(request.idcard), request.idcard));
            keyValuePairs.Add(new KeyValuePair<string, string>(nameof(request.idcardtype), request.idcardtype));
            keyValuePairs.Add(new KeyValuePair<string, string>(nameof(request.mobile), request.mobile));
            keyValuePairs.Add(new KeyValuePair<string, string>(nameof(request.realname), request.realname));
            keyValuePairs.Add(new KeyValuePair<string, string>(nameof(request.scenecode), request.scenecode));
            HttpContent httpContent = new FormUrlEncodedContent(keyValuePairs);
            //需要给X-Ca-Nonce的值生成随机字符串，每次请求不能相同
            httpContent.Headers.Add("X-Ca-Nonce", Guid.NewGuid().ToString());
            var response = await _client.PostAsync(url, httpContent);
            var content = await response.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<AliyunResponse<BankCertificationResult>>(content);
        }
    }
}
