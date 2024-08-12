using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Framework.AspNetCoreHelper.Extensions
{
    public static class HttpRequestExtension
    {
        public static string GetReferer(this HttpRequest httpRequest)
        {
            StringValues sv = StringValues.Empty;
            if (httpRequest.Headers.TryGetValue("Referer", out sv))
            {
                return sv;
            }
            if (httpRequest.Headers.TryGetValue("referer", out sv))
            {
                return sv;
            }
            return sv;
        }


        /// <summary>
        /// 由于框架中的scheme在nginx转发过来时可能不正确，所以与nginx反代协定了[X-Forwarded-Proto]为Scheme的真值，
        /// 所以该方法隐藏了访问方式中Scheme的差异，统一提供正确的Scheme
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static string GetScheme(this HttpRequest httpRequest, out bool isOrigin)
        {
            string scheme = httpRequest.Scheme;
            isOrigin = true;
            if (httpRequest.Headers.TryGetValue("X-Forwarded-Proto", out StringValues proto))
            {
                scheme = proto.ToString();
                isOrigin = false;
            }
            else if (httpRequest.Headers.TryGetValue("X-Original-Proto", out proto))
            {
                scheme = proto.ToString();
                isOrigin = false;
            }
            return scheme;
        }

        public static string FullUrlByHeader(this HttpRequest httpRequest, string app, string path)
        {
            string scheme = GetScheme(httpRequest, out bool isOrigin);

            string host = httpRequest.Host.ToString();

            if (isOrigin)
                return $"{scheme}://{host}/{path}";
            return $"{scheme}://{host}/{app}/{path}";
        }
    }
}
