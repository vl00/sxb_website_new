using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Recommend.API.RequestModels;
using Sxb.Recommend.Application.Services;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Recommend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedirectInsideController : ControllerBase
    {
        ILogger<RedirectInsideController> _logger;
        ISchoolRedirectInsideService _schoolRedirectInsideService;
        IArticleRedirectInsideService _articleRedirectInsideService;

        public RedirectInsideController(ISchoolRedirectInsideService schoolRedirectInsideService
            , ILogger<RedirectInsideController> logger
            , IArticleRedirectInsideService articleRedirectInsideService)
        {
            _logger = logger;
            _schoolRedirectInsideService = schoolRedirectInsideService;
            _articleRedirectInsideService = articleRedirectInsideService;
        }

        [HttpGet("Test")]
        public ResponseResult Test()
        {
            return ResponseResult.Success();
        }

        [HttpGet("List/PrimaryId/{primaryId}")]
        public async Task<ResponseResult> TestAsync(Guid primaryId)
        {
            var data = await _schoolRedirectInsideService.ListAsync(primaryId);
            return ResponseResult.Success(data);
        }


        [HttpPost("Add/School/Id")]
        [Description("新增学校跳转记录")]
        public async Task<ResponseResult> AddSchoolRedirect([FromBody] IdRedirectAddRequestDto requestDto)
        {
            try
            {
                await _schoolRedirectInsideService.Add(new SchoolRedirectInside(requestDto.ReferId, requestDto.CurrentId, DateTime.Now));
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "add");
                return ResponseResult.Failed(ex.Message);
            }
        }


        [HttpPost("Add/Article/Id")]
        [Description("新增文章跳转记录")]
        public async Task<ResponseResult> AddArticleRedirect([FromBody] IdRedirectAddRequestDto requestDto)
        {
            try
            {
                await _articleRedirectInsideService.Add(new ArticleRedirectInside(requestDto.ReferId,requestDto.CurrentId,DateTime.Now));
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "add");
                return ResponseResult.Failed(ex.Message);
            }
        }

        [HttpPost("Add/School/ShortId")]
        [Description("新增学校跳转记录")]
        public async Task<ResponseResult> AddSchoolRedirect([FromBody] ShortIdRedirectAddRequestDto requestDto)
        {
            try
            {
                await _schoolRedirectInsideService.Add(requestDto.ReferShortId, requestDto.CurrentShortId);
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "add");
                return ResponseResult.Failed(ex.Message);
            }
        }

        [HttpPost("Add/Article/ShortId")]
        [Description("新增文章跳转记录")]
        public async Task<ResponseResult> AddArticleRedirect([FromBody] ShortIdRedirectAddRequestDto requestDto)
        {
            try
            {
                await _articleRedirectInsideService.Add(requestDto.ReferShortId, requestDto.CurrentShortId);
                return ResponseResult.Success() ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "add");
                return ResponseResult.Failed(ex.Message);
            }
        }
    }
}
