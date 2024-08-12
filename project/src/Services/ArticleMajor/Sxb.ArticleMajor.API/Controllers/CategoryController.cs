using Microsoft.AspNetCore.Mvc;
using Sxb.ArticleMajor.API.Application.Query;
using Sxb.ArticleMajor.API.Application.Query.ViewModel;
using Sxb.ArticleMajor.API.Utils;
using Sxb.ArticleMajor.Common;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.QueryDto;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.API.Controllers
{
    [Route("[Controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryQuery _categoryQuery;

        public CategoryController(ICategoryQuery categoryQuery)
        {
            _categoryQuery = categoryQuery;
        }

        /// <summary>
        /// 获取分类名称
        /// </summary>
        /// <param name="platform">子站id</param>
        /// <param name="shortCategoryName">分类短id</param>
        /// <returns></returns>
        [Route("name")]
        [ProducesResponseType(typeof(ArticleCategoryVM), 200)]
        public async Task<ResponseResult> GetName(ArticlePlatform platform, string shortCategoryName)
        {
            var result = await _categoryQuery.GetCategoryAsync(platform, shortCategoryName);
            if (result == null) return ResponseResult.Failed();
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 获取子站所有分类
        /// </summary>
        /// <param name="platform">子站id</param>
        /// <param name="shortCategoryName">获取类型短名称的所有子, 为空则获取所有</param>
        /// <param name="shortCityName">获取指定城市的类型, 为空获取所有</param>
        /// <returns></returns>
        [Route("list")]
        [ProducesResponseType(typeof(CategoryQueryDto), 200)]
        public async Task<ResponseResult> GetCategoriesAsync(ArticlePlatform platform, string shortCategoryName = null, string shortCityName = null)
        {
            var result = await _categoryQuery.GetCategoriesAsync(platform, shortCategoryName, shortCityName);
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 获取侧边栏导航
        /// </summary>
        /// <param name="platform">子站id</param>
        /// <param name="shortCategoryName">获取类型短名称的所有子, 为空则获取所有</param>
        /// <param name="shortCityName">获取指定城市的类型, 为空获取所有</param>
        /// <returns></returns>
        [Route("list/sidebar")]
        [ProducesResponseType(typeof(CategoryQueryDto), 200)]
        public async Task<ResponseResult> GetSidebarCategoriesAsync(ArticlePlatform platform, string shortCategoryName = null, string shortCityName = null)
        {
            var data = await _categoryQuery.GetCategoriesAsync(platform, shortCategoryName, shortCityName);

            var result = data.Select(s =>
                        {
                            s.Children = new List<CategoryQueryDto>() {
                                CategorySidebarVM.GetSchoolSidebar(s.Platform)
                            }.Concat(s.Children);
                            return s;
                        }).ToList();
            return ResponseResult.Success(result);
        }

    }
}
