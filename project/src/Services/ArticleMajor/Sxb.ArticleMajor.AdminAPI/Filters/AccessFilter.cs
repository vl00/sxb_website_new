using iSchool.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sxb.ArticleMajor.AdminAPI.Filters
{
    /// <summary>
    /// 访问过滤器
    /// </summary>
    public class AccessFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {



        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var userInfo = new Account().Info(context.HttpContext);
            var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<AccessFilter>)) as ILogger;
            logger.LogInformation(" User {userId} Acess", userInfo?.Id);
        }
    }
}
