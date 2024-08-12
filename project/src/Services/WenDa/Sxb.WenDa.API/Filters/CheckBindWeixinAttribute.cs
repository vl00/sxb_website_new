using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.Common.OtherAPIClient.User;
using System.Diagnostics;

namespace Sxb.WenDa.API.Filters
{
    /// <summary>
    /// 检查当前登录用户是否绑定weixin
    /// </summary>
    public class CheckBindWeixinAttribute : ActionFilterAttribute, IAsyncActionFilter
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

            var unionIdDto = await userApiService.GetUserWxUnionId(userInfo.UserId.ToString());
            var isBind = !string.IsNullOrEmpty(unionIdDto?.UnionId);
            if (!isBind)
            {
                var r = ResponseResult.Failed("未绑定微信");
                r.status = ResponseCode.NotBindWeixin;
                context.Result = new JsonResult(r);
                return;
            }

            await next();
        }
    }
}
