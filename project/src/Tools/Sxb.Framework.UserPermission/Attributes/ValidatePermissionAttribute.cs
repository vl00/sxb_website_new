﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Framework.UserPermission.Attributes
{
    /// <summary>
    /// 启用权限验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ValidatePermissionAttribute : AuthorizeAttribute, IAuthorizationFilter, IAsyncAuthorizationFilter
    {
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            await PermissionAuthorization(context);
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await PermissionAuthorization(context);
        }

        private async Task PermissionAuthorization(AuthorizationFilterContext context)
        {
            //排除匿名访问
            if (context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(AllowAnonymousAttribute)))
                return;

            //登录验证
            //var user = context.HttpContext.RequestServices.GetService<IUser>();
            //if (user == null || !(user?.Id > 0))
            //{
            //    context.Result = new ChallengeResult();
            //    return;
            //}

            //权限验证
            var httpMethod = context.HttpContext.Request.Method;
            var api = context.ActionDescriptor.AttributeRouteInfo.Template;
            //var permissionHandler = context.HttpContext.RequestServices.GetService<IPermissionHandler>();
            var isValid = true;
                //await permissionHandler.ValidateAsync(api, httpMethod);
            if (!isValid)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
