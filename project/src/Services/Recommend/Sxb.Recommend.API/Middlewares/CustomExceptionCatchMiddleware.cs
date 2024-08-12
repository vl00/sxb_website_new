using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Recommend.API.Middlewares
{
    public class CustomExceptionCatchMiddleware
    {
        private readonly RequestDelegate _next;

        ILogger<CustomExceptionCatchMiddleware> _logger;

        public CustomExceptionCatchMiddleware(RequestDelegate next,ILogger<CustomExceptionCatchMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                await context.Response.WriteAsJsonAsync(ResponseResult.Failed(ResponseCode.Error));

            }

        }
    }

    public static class CustomExceptionCatchMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionCatch(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionCatchMiddleware>();
        }
    }

}
