using DotNetCore.CAP;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.API.Application.Commands;
using Sxb.WenDa.API.Application.IntegrationEventHandlers;
using Sxb.WenDa.API.Application.IntegrationEvents;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Filters;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.ElasticSearch.Base;
using Sxb.WenDa.Query.ElasticSearch.Models;
using Sxb.WenDa.Query.ElasticSearch.QueryModels;

namespace Sxb.WenDa.API.Controllers
{

    /// <summary>
    /// 提醒
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NotifyController : ApiControllerBase
    {
        private readonly ICapPublisher _capPublisher;

        public NotifyController(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        /// <summary>
        /// 推送所有提醒
        /// </summary>
        /// <returns></returns>        
        [HttpGet("test")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        public async Task<ResponseResult> NotifyTestAsync([FromServices] INotifyIntegrationEventHandler handler, bool isTest = false)
        {
            await handler.SendAsync(new NotifyIntegrationEvent()
            {
                Time = DateTime.Now,
                IsTest = isTest
            });
            return ResponseResult.Success();
        }

        /// <summary>
        /// 推送所有提醒
        /// </summary>
        /// <returns></returns>        
        [HttpGet("all")]
        [ProducesResponseType(typeof(ResponseResult), 200)]
        public async Task<ResponseResult> NotifyAsync()
        {
            await _capPublisher.PublishAsync(nameof(NotifyIntegrationEvent), new NotifyIntegrationEvent()
            {
                Time = DateTime.Now,
            });
            return ResponseResult.Success();
        }



    }
}
