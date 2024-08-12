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
    /// 城市接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ApiControllerBase
    {
        private readonly ICityCategoryQuery _cityCategoryQuery;

        public CitiesController(ICityCategoryQuery cityCategoryQuery)
        {
            _cityCategoryQuery = cityCategoryQuery;
        }

        /// <summary>
        /// 获取开放的城市列表
        /// </summary>
        /// <returns></returns>  
        [HttpGet("open")]
        [ProducesResponseType(typeof(CityDto), 200)]
        public async Task<ResponseResult> GetHotSubjects()
        {
            var data = await _cityCategoryQuery.GetCitys();
            return ResponseResult.Success(data);
        }
    }
}
