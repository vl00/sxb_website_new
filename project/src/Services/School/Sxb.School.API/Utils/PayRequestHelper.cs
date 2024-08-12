using System;
using System.Net.Http;

namespace Sxb.School.API.Utils
{
    public static class PayRequestHelper
    {
        /// <summary>
        /// 支付中心签名header
        /// </summary>
        /// <returns></returns>
        public static HttpContent SetSignHeader(HttpContent content, string paykey, string body, string system)
        {
            var timespan = DateTime.Now.ToString("yyyyMMddHHmmss");
            var nonce = Guid.NewGuid().ToString("N");
            content.Headers.TryAddWithoutValidation("sxb.timespan", timespan);
            content.Headers.TryAddWithoutValidation("sxb.nonce", nonce);
            content.Headers.TryAddWithoutValidation("sxb.key", system);
            var sign = $"{paykey}{timespan}\n{nonce}\n{body}\n";
            content.Headers.TryAddWithoutValidation("sxb.sign", HashAlgmUtil.Encrypt(sign, "md5", false));
            return content;
        }

    }
}
