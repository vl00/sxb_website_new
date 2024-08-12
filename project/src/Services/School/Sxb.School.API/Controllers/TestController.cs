using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.School.API.Application.Commands;
using Sxb.School.API.Application.Models;
using Sxb.School.API.Application.Queries.DgAyOrder;
using Sxb.School.API.Application.Queries.DegreeAnalyze;
using Sxb.School.API.Infrastructures.Services;
using Sxb.School.API.Infrastructures.Services.Models;
using Sxb.School.API.Models;
using Sxb.School.API.RequestContact.DegreeAnalyze;
using Sxb.School.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sxb.School.API.Application.Queries.UserInfo;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using Sxb.School.API.Filters;
using System.Security.Claims;

namespace Sxb.School.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        ILogger<DegreeAnalyzeController> _logger;
        readonly IEasyRedisClient _easyRedisClient;
        readonly IMediator _mediator;
        readonly IDegreeAnalyzeQueries _degreeAnalyzeQueries;
        readonly IHostEnvironment _hostEnvironment;

        public TestController(IEasyRedisClient easyRedisClient
            , IDegreeAnalyzeQueries degreeAnalyzeQueries
            , IMediator mediator
            , ILogger<DegreeAnalyzeController> logger
            , IHostEnvironment hostEnvironment)
        {
            _easyRedisClient = easyRedisClient;
            this._mediator = mediator;
            this._degreeAnalyzeQueries = degreeAnalyzeQueries;
            _logger = logger;
            this._hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        [ProducesResponseType(typeof(APIResult<string>), 200)]
        public ResponseResult Err(int code)
        {
            throw new ResponseResultException("测试throw ResponseResultException", code);
        }

        /// <summary>
        /// 获取题目s
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(APIResult<DgAyGetQuestionResponse>), 200)]
        public async Task<ResponseResult> DgAyGetQuestionsFromDB()
        {
            var r = await _degreeAnalyzeQueries.GetQuestions(readCache: false, writeCache: false, showFindField: true);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// 查看(我和别人的)分析报告（含未解锁和已解锁）
        /// </summary>
        /// <param name="id">
        /// 报告id.<br/>
        /// showAll=0并且includeDeletedSchool=0时,相当于原接口看被人的报告
        /// </param>
        /// <param name="showAll">1=忽略解锁状态查看报告</param>
        /// <param name="includeDeletedSchool">是否包括被删除的学校</param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(APIResult<DgAyQaResultVm>), 200)]
        public async Task<ResponseResult> DgAyMyQa(Guid id, int showAll = 1, int includeDeletedSchool = 1)
        {
            var r = await _degreeAnalyzeQueries.GetQaResult(id, showAll: showAll != 0, includeDeletedSchool: includeDeletedSchool != 0);
            return ResponseResult.Success(r);
        }

        /// <summary>
        /// test get curr user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<string> GetUser()
        {
            await default(ValueTask);
            var idd = HttpContext.User.Identity as ClaimsIdentity;
            var s = $"HttpContext.User.Identity.GetType() = {(HttpContext.User.Identity?.GetType()?.FullName ?? "null")}";
            s += "\n\n" + $"HttpContext.User.Identity.IsAuthenticated = {(idd?.IsAuthenticated.ToString()?.ToLower() ?? "null")}";
            s += "\n\n" + $"UserId = {(idd?.FindFirst("id")?.Value ?? "null")}";
            s += "\n\n" + $"Cookies-iSchoolAuth = {(HttpContext.Request.Cookies["iSchoolAuth"] ?? "null")}";
            return s;
        }
    }


}
