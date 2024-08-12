using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.PointsMall.API.Config;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sxb.PointsMall.API.Infrastructure.Filter
{
    public class SignatureFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// userid是否参与签名
        /// </summary>
        public bool SignatureUserId { get; set; }
        public SignatureFilterAttribute(bool signatureUserId = false)
        {
            SignatureUserId = signatureUserId;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var request = httpContext.Request;


            List<KeyValuePair<string, object>> keyValuePairs = null;
            if (request.Method == "GET")
            {
                var query = context.HttpContext.Request.Query;
                keyValuePairs = query.Select(x => new KeyValuePair<string, object>(x.Key, x.Value.ToString())).ToList();
            }
            else
            {
                var body = new StreamReader(request.Body)
                    .ReadToEndAsync().GetAwaiter().GetResult();
                keyValuePairs = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, object>>>(body).ToList();
            }

            if (keyValuePairs != null && keyValuePairs.Any(s => s.Key == "signature"))
            {
                if (SignatureUserId)
                {
                    var userId = context.HttpContext.User.Identity.GetID();
                    keyValuePairs.Add(new KeyValuePair<string, object>("userId", userId));
                }
                keyValuePairs.Add(new KeyValuePair<string, object>("secret", Configs.GetTaskRSATail()));

                var signature = keyValuePairs.FirstOrDefault(s => s.Key == "signature").Value?.ToString();
                var _signature = Signature(keyValuePairs.Where(s => s.Key != "signature"));
                //test
                Debug.WriteLine("测试签名:" + _signature);

                if (signature?.ToLower() == _signature?.ToLower())
                {
                    //success
                    base.OnActionExecuting(context);
                    return;
                }
            }

            //fail
            context.Result = new JsonResult(new ResponseResult()
            {
                Succeed = false,
                status = ResponseCode.ValidationError,
                Msg = "签名错误"
            });
        }


        public string Signature(IEnumerable<KeyValuePair<string, object>> paras)
        {
            var parasStr = paras.OrderBy(s => s.Key).Select(s => s.Key + "=" + s.Value).ToArray();
            string source = string.Join("&", parasStr);
            return MD5Helper.GetMD5(source, _MD5Tail: string.Empty);
        }


        public string RSAEncrypt(params string[] paras)
        {
            string source = string.Join("", paras.OrderBy(s => s).ToArray()) + Configs.GetTaskRSATail();
            return RSAHelper.RSAEncrypt(Configs.GetTaskRSAPublicKey(), source);
        }

        public string RSADecrypt(string signature)
        {
            try
            {
                return RSAHelper.RSADecrypt(Configs.GetTaskRSAPrivateKey(), signature);
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }
    }
}
