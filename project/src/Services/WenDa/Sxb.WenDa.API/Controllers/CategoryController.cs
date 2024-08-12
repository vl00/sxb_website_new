using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.API.Application.Commands;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Filters;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.Enum;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.ElasticSearch.Models;
using Sxb.WenDa.Query.ElasticSearch.QueryModels;

namespace Sxb.WenDa.API.Controllers
{

    /// <summary>
    /// 分类
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ApiControllerBase
    {
        private readonly ICityCategoryQuery _cityCategoryQuery;

        public CategoryController(ICityCategoryQuery cityCategoryQuery)
        {
            _cityCategoryQuery = cityCategoryQuery;
        }


        /// <summary>
        /// 子站及其一级子分类
        /// </summary>
        /// <returns></returns>        
        [HttpGet("platform/children")]
        [ProducesResponseType(typeof(CategoryChildDto), 200)]
        public async Task<ResponseResult> GetPlatformChildren(ArticlePlatform platform, int city = 440100)
        {
            var data = await _cityCategoryQuery.GetPlatformWithChildren(platform ,city);
            return ResponseResult.Success(data);
        }


        /// <summary>
        /// 获取子站的一二级分类
        /// </summary>
        /// <returns></returns>        
        [HttpGet("platform/12")]
        [ProducesResponseType(typeof(CategoryChildDto), 200)]
        public async Task<ResponseResult> GetChildren(ArticlePlatform platform, int city = 440100)
        {
            var data = await _cityCategoryQuery.GetChildren(platform, city);
            return ResponseResult.Success(data);
        }

    }
}
