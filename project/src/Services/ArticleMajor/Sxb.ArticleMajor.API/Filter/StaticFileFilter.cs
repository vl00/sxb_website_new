using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.API.Filter
{
    public class StaticFileFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 生成路径，相对根路径，范例：/Typings/app/app.component.html
        /// </summary>
        public string Path { get; set; }

        public StaticFileFilter()
        {
            Path = "wwwroot/html-files/article-detail";
        }

        public string GetFilePath(ActionContext context)
        {
            var fileNameOnly = GetFileNameOnly(context);
            var fileName = fileNameOnly + ".html";
            return System.IO.Path.Combine(Path, fileName);
        }

        public string GetFileNameOnly(ActionContext context)
        {
            if (context.HttpContext.Request.Query.TryGetValue("id", out StringValues values))
            {
                return values.ToString();
            }
            throw new Exception("unhandle id");
        }

        /// <summary>
        /// 执行生成
        /// </summary>
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            await WriteViewToFileAsync(context);
            await base.OnResultExecutionAsync(context, next);
        }

        /// <summary>
        /// 将视图写入html文件
        /// </summary>
        private async Task WriteViewToFileAsync(ResultExecutingContext context)
        {
            try
            {
                var html = await RenderToStringAsync(context);
                if (string.IsNullOrWhiteSpace(html))
                    return;

                var directory = System.IO.Path.GetDirectoryName(Path);
                if (string.IsNullOrWhiteSpace(directory))
                    return;
                if (Directory.Exists(directory) == false)
                    Directory.CreateDirectory(directory);
                File.WriteAllText(GetFilePath(context), html);
            }
            catch (Exception ex)
            {
                IServiceProvider serviceProvider = context.HttpContext.RequestServices;
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.CreateLogger<StaticFileFilter>().LogError(ex, "生成html静态文件失败");
            }
        }

        /// <summary>
        /// 渲染视图
        /// </summary>
        protected async Task<string> RenderToStringAsync(ResultExecutingContext context)
        {
            string viewName = "";
            ViewDataDictionary viewDictionary = null;
            if (context.Result is ViewResult result)
            {
                viewName = result.ViewName;
                viewName = string.IsNullOrWhiteSpace(viewName) ? context.RouteData.Values["action"].ToString() : viewName;
                viewDictionary = result.ViewData;
            }
            IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            IRazorViewEngine razorViewEngine = serviceProvider.GetService<IRazorViewEngine>();
            ITempDataProvider tempDataProvider = serviceProvider.GetService<ITempDataProvider>();

            var actionContext = context;

            //var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
            //new新对象viewResult.View.RenderAsync会报错Could not find an IRouter associated with the ActionContext.
            //var actionContext =  new ActionContext(httpContext, context.RouteData, new ActionDescriptor());

            using (var stringWriter = new StringWriter())
            {
                var viewResult = razorViewEngine.FindView(actionContext, viewName, true);
                if (viewResult.View == null)
                    throw new ArgumentNullException($"未找到视图： {viewName}");

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                    stringWriter,
                    new HtmlHelperOptions());

                await viewResult.View.RenderAsync(viewContext);
                return stringWriter.ToString();
            }
        }
    }
}
