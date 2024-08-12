using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Sxb.ArticleMajor.API.Application.Query;
using Sxb.ArticleMajor.API.Application.Query.RequestModel;
using Sxb.ArticleMajor.API.Application.Query.ViewModel;
using Sxb.ArticleMajor.Common;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.API.Application.IntegrationEvents;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.API.Controllers
{
    [Route("[Controller]")]
    public class ArticleController : Controller
    {

        private readonly IArticleQuery _articleQuery;
        private readonly ICategoryQuery _categoryQuery;

        private readonly ICapPublisher _capPublisher;

        public ArticleController(ICategoryQuery categoryQuery, IArticleQuery articleQuery, ICapPublisher capPublisher)
        {
            _categoryQuery = categoryQuery;
            _articleQuery = articleQuery;
            _capPublisher = capPublisher;
        }


        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="code">文章编码</param>
        /// <param name="page">页面, 默认1</param>
        /// <returns></returns>
        [Route("detail")]
        [ProducesResponseType(typeof(ArticleDetailVM), 200)]
        public async Task<ResponseResult> DetailAsync(string code, int page = 1)
        {
            var result = await _articleQuery.GetArticleDetailAsync(code, page);
            if (result != null)
            {
                //增加阅读量
                await _capPublisher.PublishAsync(nameof(ViewArticleIntegrationEvent)
                    , new ViewArticleIntegrationEvent(result.Id, DateTime.Now, null));
                return ResponseResult.Success(result);
            }
            return ResponseResult.Failed();
        }

        /// <summary>
        /// 获取子站分类下的最新文章
        /// </summary>
        /// <param name="platform">子站  1幼儿教育 2小学教育 3中学教育 4中职网 5高中教育 6素质教育 7国际教育</param>
        /// <param name="cityShortName">选填,城市短地址</param>
        /// <param name="categoryShortName">选填,类型短地址</param>
        /// <param name="top">选填,数量,默认10</param>
        /// <returns></returns>
        [Route("lastest")]
        [ProducesResponseType(typeof(ArticleItemVM), 200)]
        public async Task<ResponseResult> GetLastestArticles(ArticlePlatform platform, string cityShortName, string categoryShortName, int top = 10)
        {
            bool mustCover = categoryShortName.Contains("czzl") || categoryShortName.Contains("xybg");
            var result = await _articleQuery.GetLastestArticles(platform, cityShortName, categoryShortName, top, mustCover);
            return ResponseResult.Success(result);
        }


        /// <summary>
        /// 获取子站分类下的热门文章
        /// </summary>
        /// <param name="platform">子站  1幼儿教育 2小学教育 3中学教育 4中职网 5高中教育 6素质教育 7国际教育</param>
        /// <param name="cityShortName">选填,城市短地址</param>
        /// <param name="categoryShortName">选填,类型短地址</param>
        /// <param name="top">选填,数量,默认10</param>
        /// <returns></returns>
        [Route("hot")]
        [ProducesResponseType(typeof(ArticleItemVM), 200)]
        public async Task<ResponseResult> GetHotArticles(ArticlePlatform platform, string cityShortName, string categoryShortName, int top = 10)
        {
            var result = await _articleQuery.GetLastestArticles(platform, cityShortName, categoryShortName, top);
            return ResponseResult.Success(result);
        }


        /// <summary>
        /// 推荐文章列表
        /// 根据分类名称相同来推荐
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="cityId"></param>
        /// <param name="categoryName"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        [Route("recommends")]
        [ProducesResponseType(typeof(ArticleItemVM), 200)]
        public async Task<ResponseResult> GetRecommendArticles(ArticlePlatform platform, int cityId, string categoryName, int top = 10)
        {
            var result = await _articleQuery.GetRecommendArticles(platform, cityId, categoryName, top);
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 获取文章分页列表
        /// </summary>
        /// <param name="reqDto"></param>
        /// <returns></returns>
        [Route("pagination")]
        [ProducesResponseType(typeof(PaginationArticleVM<ArticleItemVM>), 200)]
        public async Task<ResponseResult> GetPaginationAsync([FromQuery]ArticlesPaginationReqDto reqDto)
        {
            var result = await _articleQuery.GetPaginationAsync(reqDto);
            return ResponseResult.Success(result);
        }
    }
}
