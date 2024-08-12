using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System;
using System.Threading.Tasks;

namespace Sxb.WenDa.API.Filters
{
    /// <summary>
    /// 吧抛出的 ResponseResultException 转为 ResponseResult.Failed() 返回前端
    /// </summary>
    public class ResponseResultExceptionToResultAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            var exception = context.Exception;
            if (exception is ResponseResultException rex)
            {
                var r = ResponseResult.Failed((ResponseCode)rex.Code, rex.Message, null);
                context.Result = new ObjectResult(r);
            }
        }
    }
}
