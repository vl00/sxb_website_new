using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.OtherAPIClient.User;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

namespace Sxb.WenDa.API.Filters
{
    /// <summary>
    /// 检查当前登录用户是否绑定手机号
    /// </summary>
    public class CheckBindMobileAttribute : ActionFilterAttribute, IAsyncActionFilter
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var HttpContext = context.HttpContext;
            var services = context.HttpContext.RequestServices;
            var config = services.GetService<IConfiguration>();
            var log = services.GetService<ILoggerFactory>().CreateLogger(this.GetType());
            var userApiService = services.GetService<IUserApiService>();

            Debug.Assert(HttpContext.User.Identity.IsAuthenticated);
            var userInfo = HttpContext.GetUserInfo();

            // 貌似cookie里有非完整的用户手机号
            var isBind = !string.IsNullOrEmpty(HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.FindFirst("phone_number")?.Value : null);
            if (!isBind)
            {
                // 查接口
                isBind = await userApiService.IsUserBindMobile(userInfo.UserId);
                if (!isBind)
                {
                    var r = ResponseResult.Failed("未绑定手机号");
                    r.status = ResponseCode.NotBindMobile;
                    context.Result = new JsonResult(r);
                    return;
                }
            }

            await next();
        }
    }
}
