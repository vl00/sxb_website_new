using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using RazorEngine.Templating;
using Sxb.ArticleMajor.API.Application.Query;
using Sxb.ArticleMajor.API.Filter;
using Sxb.ArticleMajor.API.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using static RazorEngine.Templating.RazorEngineServiceExtensions;

namespace Sxb.ArticleMajor.API.Controllers
{
    public class ArticleTestController : Controller
    {
        private readonly ILogger<ArticleTestController> _logger;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly RazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly IArticleQuery _articleQuery;

        public ArticleTestController(ILogger<ArticleTestController> logger, IRazorViewEngine razorViewEngine, RazorViewToStringRenderer razorViewToStringRenderer, IArticleQuery articleQuery)
        {
            _logger = logger;
            _razorViewEngine = razorViewEngine;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _articleQuery = articleQuery;
        }

        public async Task<IActionResult> DetailAsync(string code, int page = 1)
        {
            var article = await _articleQuery.GetArticleDetailAsync(code, page);
            return new JsonResult(article);
        }


        //[StaticFileFilter()]
        public async Task<IActionResult> IndexAsync(long id)
        {
            ViewBag.Id = id;

            //for (int i = 0; i < 10; i++)
            //{
            //    await new TemplateUtils().BuildAsync(i.ToString(), TemplateUtils.TestContent);
            //}

            //var html = await _razorViewToStringRenderer.RenderViewToStringAsync(ControllerContext, "Views/Article/List.cshtml", (object)null);

            return View();
        }
        public IActionResult List()
        {
            return View();
        }


        public void RazorRunTest(long id)
        {
            var dic = ViewBagToDic(ViewBag);

            var filename = $"wwwroot/html-files/article-detail/{id}.html";
            using TextWriter sw = System.IO.File.CreateText(filename);
            RazorEngine.Engine.Razor.RunCompile("Index", sw, modelType: (Type)null, model: (object)null, viewBag: new DynamicViewBag(dic));
            sw.Flush();
        }

        public IDictionary<string, object> ViewBagToDic(dynamic viewBag)
        {
            var dic = new Dictionary<string, object>();
            if (viewBag == null)
            {
                return dic;
            }

            Type type = viewBag.GetType();// typeof(DynamicObject);
            BindingFlags instanceBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var viewDataProp = type.GetProperty("ViewData", instanceBindFlags);
            var viewData = viewDataProp.GetValue(viewBag, null) as ViewDataDictionary;
            return viewData;
        }
    }
}