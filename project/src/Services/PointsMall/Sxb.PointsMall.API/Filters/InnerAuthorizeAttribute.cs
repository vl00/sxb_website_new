using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Filters
{
    public class InnerAuthorizeAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers.TryGetValue("sxb-innerToken", out StringValues value))
            {
                if (!value.ToString().Equals("sxb-yyds", StringComparison.CurrentCultureIgnoreCase))
                {
                    context.Result = new JsonResult(ResponseResult.Failed(ResponseCode.NoAuth, null));
                }

            }
            else {
                context.Result = new JsonResult(ResponseResult.Failed(ResponseCode.NoAuth, null));
            }
            base.OnActionExecuting(context);
        }

    }
}
