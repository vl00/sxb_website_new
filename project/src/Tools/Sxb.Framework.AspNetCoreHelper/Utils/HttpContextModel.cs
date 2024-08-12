using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Sxb.Framework.AspNetCoreHelper.Utils
{
    public class HttpContextModel
    {
        public static IApplicationBuilder ApplicationBuilder { get; set; }
        public static IHttpContextAccessor HttpContextAccessor => ApplicationBuilder.ApplicationServices.GetService<IHttpContextAccessor>();
        public static HttpContext HttpContext => HttpContextAccessor.HttpContext;

        public static IServiceScopeFactory GetServiceScopeFactory() => ApplicationBuilder.ApplicationServices.GetService<IServiceScopeFactory>();
    }
}
