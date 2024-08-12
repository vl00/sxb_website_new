using Microsoft.AspNetCore.Builder;
using Sxb.Framework.AspNetCoreHelper.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Framework.AspNetCoreHelper.Extensions
{
    public static class AppExtension
    {
        public static IApplicationBuilder AddHttpContextModel(this IApplicationBuilder builder)
        {
            HttpContextModel.ApplicationBuilder = builder;
            return builder;
        }
    }
}
