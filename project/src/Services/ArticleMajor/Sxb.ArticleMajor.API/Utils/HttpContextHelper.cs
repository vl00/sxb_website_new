using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.API.Utils
{
    public class HttpContextHelper
    {
        public static IApplicationBuilder ApplicationBuilder { get; set; }
        public static IHttpContextAccessor HttpContextAccessor => ApplicationBuilder.ApplicationServices.GetService<IHttpContextAccessor>();
    }
}
