using Microsoft.AspNetCore.Mvc;
using Sxb.ArticleMajor.AdminAPI.ActionContract.Keyword;
using Sxb.ArticleMajor.AdminAPI.Application.Query;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;

namespace Sxb.ArticleMajor.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordController : ControllerBase
    {
        readonly IKeywordQuery _keywordQuery;

        public KeywordController(IKeywordQuery keywordQuery)
        {
            _keywordQuery = keywordQuery;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="sityType">网站类型</param>
        /// <param name="cityCode">城市代码</param>
        /// <param name="pageType">页面类型</param>
        /// <param name="positionType">位置类型</param>
        /// <returns></returns>
        [HttpGet]
        [Route("List")]
        public async Task<ResponseResult> List(KeywordSiteType sityType, int? cityCode, KeywordPageType pageType, KeywordPositionType positionType)
        {
            var result = ResponseResult.DefaultFailed();
            var finds = await _keywordQuery.ListAsync(sityType, cityCode, pageType, positionType);
            if (finds?.Any() == true) result = ResponseResult.Success(finds);
            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Remove")]
        public async Task<ResponseResult> Remove(Guid id)
        {
            var result = ResponseResult.DefaultFailed();
            if (await _keywordQuery.RemoveAsync(id)) result = ResponseResult.Success();
            return result;
        }

        [HttpPost]
        [Route("Save")]
        public async Task<ResponseResult> Save(SaveRequest request)
        {
            var result = ResponseResult.DefaultFailed();
            if (request == default) return result;

            return result;
        }
    }
}
