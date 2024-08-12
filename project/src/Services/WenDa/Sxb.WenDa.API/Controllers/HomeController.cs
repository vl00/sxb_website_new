using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sxb.WenDa.Common.Enum;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.API.Application.Commands;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Filters;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Common.ResponseDto.Home;
using Sxb.WenDa.Query.ElasticSearch.Models;
using Sxb.WenDa.Query.ElasticSearch.QueryModels;

namespace Sxb.WenDa.API.Controllers
{

    /// <summary>
    /// 主页接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ApiControllerBase
    {
        private readonly ILanmuQuery _lanmuQuery;
        private readonly IQuestionQuery _questionQuery;
        private readonly ICityCategoryQuery _cityCategoryQuery;

        public HomeController(ILanmuQuery lanmuQuery, IQuestionQuery questionQuery, ICityCategoryQuery cityCategoryQuery)
        {
            _lanmuQuery = lanmuQuery;
            _questionQuery = questionQuery;
            _cityCategoryQuery = cityCategoryQuery;
        }

        /// <summary>
        /// 获取主站热门专栏
        /// </summary>
        /// <returns></returns>  
        [HttpGet("hot/subjects")]
        [ProducesResponseType(typeof(HotSubjectItemDto), 200)]
        public async Task<ResponseResult> GetHotSubjects()
        {
            var data = await _lanmuQuery.GetHotSubjects(ArticlePlatform.Master);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 获取子站热门专栏
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="city"></param>
        /// <returns></returns>  
        [HttpGet("sub/hot/subjects")]
        [ProducesResponseType(typeof(SubHotSubjectItemDto), 200)]
        public async Task<ResponseResult> GetSubHotSubjects(ArticlePlatform platform, int? city)
        {
            city = null;//产品说:子站热门专栏暂时无需按城市配置
            var data = await _lanmuQuery.GetSubHotSubjects(platform, city);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 获取子站分类和他的问题
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="city"></param>
        /// <returns></returns>  
        [HttpGet("sub/categories")]
        [ProducesResponseType(typeof(SubCategoryItemDto), 200)]
        public async Task<ResponseResult> GetSubCategories(ArticlePlatform platform, int city)
        {
            var data = await _lanmuQuery.GetSubCategories(platform, city);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 获取大家热议
        /// </summary>
        /// <returns></returns>        
        [HttpGet("hot/questions")]
        [ProducesResponseType(typeof(QuestionLinkDto), 200)]
        public async Task<ResponseResult> GetHotQuestions()
        {
            var data = await _lanmuQuery.GetHotQuestions();
            return ResponseResult.Success(data);
        }


        /// <summary>
        /// 等你来回答
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>        
        [HttpGet("wait/questions")]
        [ProducesResponseType(typeof(ToBeAnsweredQuestionListItemDto), 200)]
        public async Task<ResponseResult> GetWaitQuestions(int top = 5)
        {
            var data = await _questionQuery.GetWaitQuestions(UserId, top);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 热门学校问答
        /// 换一批pageIndex++
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="city"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("hot/schools")]
        [ProducesResponseType(typeof(HotQuestionSchoolItemDto), 200)]
        public async Task<ResponseResult> GetHotSchools(ArticlePlatform platform, int? city, int pageIndex, int pageSize)
        {
            var data = await _lanmuQuery.GetHotSchools(platform, city, pageIndex, pageSize);
            long categoryId = 0L;
            if (platform != ArticlePlatform.Master)
            {
                var category = await _cityCategoryQuery.GetSchoolCategory(platform);
                categoryId = category?.Id ?? 0;
            }

            return ResponseResult.Success(new
            {
                data,
                categoryId
            });
        }

        /// <summary>
        /// 主站热门推荐
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        [HttpGet("recommend/questions")]
        [ProducesResponseType(typeof(QuestionCategoryItemDto), 200)]
        public async Task<ResponseResult> GetRandomRecommendAsync(int top = 210)
        {
            var data = await _questionQuery.GetRandomRecommendAsync(top);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 检查已登录用户是否绑定手机和微信
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet(nameof(CheckBind))]
        [CheckBindMobile, CheckBindWeixin]
        [ProducesResponseType(typeof(APIResult<object>), 200)]
        public ResponseResult CheckBind()
        {
            return ResponseResult.Success();
        }

    }
}
