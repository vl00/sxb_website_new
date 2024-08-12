using Microsoft.Extensions.Options;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Sxb.Settlement.API.GaoDeng
{
    public class GaoDengService : IGaoDengService
    {
        GaoDengOption _option;
        HttpClient _client;
        JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        ILogger<GaoDengService> _logger;
        public GaoDengService(
            IOptions<GaoDengOption> options
            , HttpClient client
            , ILogger<GaoDengService> logger)
        {
             client.Timeout = TimeSpan.FromMinutes(1);
            _option = options.Value;
            _client = client;
            _client.BaseAddress = new Uri(_option.BaseUrl);
            _logger = logger;
        }


        /// <summary>
        /// 批量查询签约结果
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseResult<IEnumerable<AgreementResult>>> BatchQueryAgreement(BatchQueryAgreementRequest request)
        {
            string url = "/api/balance/BatchQueryAgreement";
            string reqstBody = JsonConvert.SerializeObject(request, serializerSettings);
            var responseMessage = await this.PostToGaoDeng(url, reqstBody, null);
            return JsonConvert.DeserializeObject<ResponseResult<IEnumerable<AgreementResult>>>(responseMessage);
        }

        public async Task<ResponseResult> CreateForBatch(List<SettlementCreate> createForBatchRequest)
        {
            string url = "/api/balance/CreateForBatch";
            string reqstBody = JsonConvert.SerializeObject(createForBatchRequest, serializerSettings);
            var responseMessage = await this.PostToGaoDeng(url, reqstBody, _option.SettlementStatusCallBack);
            return JsonConvert.DeserializeObject<ResponseResult>(responseMessage);
        }

        /// <summary>
        /// 查询结算单
        /// </summary>
        /// <param name="order_random_code">客户订单号</param>
        /// <param name="settlement_code">结算单号</param>
        /// <returns></returns>

        public async Task<ResponseResult<Settlement>> GetBalance(string order_random_code, string settlement_code)
        {
            string url = "/api/balance/getbalance";
            string reqstBody = JsonConvert.SerializeObject(new
            {
                order_random_code,
                settlement_code
            }, serializerSettings);
            var responseMessage = await this.PostToGaoDeng(url, reqstBody, null);
            return JsonConvert.DeserializeObject<ResponseResult<Settlement>>(responseMessage);

        }

        public Task GetIdentityAuditResult()
        {
            throw new NotImplementedException();
        }

        public string GetSignPage(UserBaseInfo userBaseInfo)
        {
            var info = new
            {
                userBaseInfo.idNumber,
                userBaseInfo.name,
                userBaseInfo.phoneNumber,
                userBaseInfo.serialNum,
                userBaseInfo.orderRandomCode,
                userBaseInfo.returnUrl,
                userBaseInfo.certificateType,
                merchantId = _option.AppKey
            };
            string data = this.Encrytion(JsonConvert.SerializeObject(info));
            string base64AppKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(_option.AppKey));
            return $"{_option.H5ToolUrl}?data={data}&appkey={base64AppKey}";
        }

        public async Task<ResponseResult> RefundBalance(List<string> settlement_code = null, List<string> order_random_code = null)
        {
            string url = "/api/balance/refundbalance";
            string reqstBody = JsonConvert.SerializeObject(new
            {
                settlement_code,
                order_random_code
            }, serializerSettings);
            var responseMessage = await this.PostToGaoDeng(url, reqstBody, _option.SettilementRefundCallBack);
            return JsonConvert.DeserializeObject<ResponseResult>(responseMessage);

        }

        public string GetSignature(string timestamp)
        {
            string signature = MD5Helper.GetHMACSHA256(_option.AppSecret, $"{_option.AppKey}{timestamp}{_option.AppSecret}");
            return signature.ToLower();
        }

        public string Encrytion(string plainText)
        {
            return AESHelper.EncryptionToBase64(plainText, _option.AppSecret, _option.AESIV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
        }

        public string Decrytion(string ciyperText)
        {
            return AESHelper.DecryptionFromBase64(ciyperText, _option.AppSecret, _option.AESIV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
        }

        async Task<string> PostToGaoDeng(string? requesturi, string data, string? callback_url)
        {
            try
            {
                int timestamp = DateTime.Now.D2ISecond();
                string signature = GetSignature(timestamp.ToString());
                string aesData = Encrytion(data);
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(new { data = aesData }), Encoding.UTF8, "application/json");
                httpContent.Headers.TryAddWithoutValidation("appkey", _option.AppKey);
                httpContent.Headers.TryAddWithoutValidation("request_id", Guid.NewGuid().ToString("N"));
                httpContent.Headers.TryAddWithoutValidation("timestamp", timestamp.ToString());
                httpContent.Headers.TryAddWithoutValidation("sign_type", "sha256");
                httpContent.Headers.TryAddWithoutValidation("signature", signature);
                httpContent.Headers.TryAddWithoutValidation("version", "2.0");
                if (!string.IsNullOrEmpty(callback_url))
                {
                    httpContent.Headers.TryAddWithoutValidation("callback_url", Convert.ToBase64String(Encoding.UTF8.GetBytes(callback_url)));
                }
                var response = await _client.PostAsync(requesturi, httpContent);
                response.EnsureSuccessStatusCode();
                string reponseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("请求体:{data}，响应体:{body}", data, reponseBody);

                return reponseBody;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "请求高灯服务发生异常，详情请查看异常信息。");
                throw new GaoDengException("请求高灯服务发生异常，详情请查看异常信息。", ex);
            }
        }
        async Task<HttpResponseMessage> GetFromGaoDeng(string? requesturi)
        {
            int timestamp = DateTime.Now.D2ISecond();
            _client.DefaultRequestHeaders.TryAddWithoutValidation("appkey", _option.AppKey);
            _client.DefaultRequestHeaders.TryAddWithoutValidation("request_id", Guid.NewGuid().ToString("N"));
            _client.DefaultRequestHeaders.TryAddWithoutValidation("appkey", _option.AppKey);
            _client.DefaultRequestHeaders.TryAddWithoutValidation("timestamp", timestamp.ToString());
            _client.DefaultRequestHeaders.TryAddWithoutValidation("sign_type", "sha256");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("signature", GetSignature(timestamp.ToString()));
            _client.DefaultRequestHeaders.TryAddWithoutValidation("version", "2.0");
            return await _client.GetAsync(requesturi);
        }

        public T CallBackDecode<T>(string callbackBody) where T:class
        {
            ResponseResult<string> ciperResult = JsonConvert.DeserializeObject<ResponseResult<string>>(callbackBody);
            if (ciperResult != null && ciperResult.code == 0)
            {
                string plaintext = this.Decrytion(ciperResult.data);
                return JsonConvert.DeserializeObject<T>(plaintext);
            }
            else {
                return default(T);
            }

        }
    }
}