using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using SKIT.FlurlHttpClient.Wechat.Work;
using SKIT.FlurlHttpClient.Wechat.Work.Events;
using Sxb.WeWorkFinance.API.Application.Commands;
using Sxb.WeWorkFinance.API.Application.Queries;
using Sxb.WeWorkFinance.API.Application.Queries.ViewModels;
using Sxb.WeWorkFinance.API.RequestModels;
using Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sxb.WeWorkFinance.API.Application.HttpClients;
using Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate;
using Sxb.WeWorkFinance.Infrastructure.Repositories;

namespace Sxb.WeWorkFinance.API.Controllers
{
    //[Route("api/[controller]/[action]")]
    [ApiController]
    public class InterfaceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IEasyRedisClient _easyRedisClient;

        public InterfaceController(IMediator mediator, IEasyRedisClient easyRedisClient)
        {
            _mediator = mediator;
            _easyRedisClient = easyRedisClient;
        }


        /// <summary>
        /// 回调接口验证
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("interface/callbackexternalcontact")]
        public async Task<string> CallbackExternalContactVaildUrl([FromQuery] CallbackRequest request)
        {
            string sToken = "x7XaZ";
            string sCorpID = "ww3f6a2088ec08814d";
            string sEncodingAESKey = "Na6hxrvfsRXlFo2TjA4alMXzYVXhar6BLlqsNj9Bl87";


            Tencent.WXBizMsgCrypt wxcpt = new Tencent.WXBizMsgCrypt(sToken, sEncodingAESKey, sCorpID);

            string sMsg = "";  // 解析之后的明文
            var ret = wxcpt.VerifyURL(request.Msg_signature, request.Timestamp, request.Nonce, request.Echostr, ref sMsg);

            return await Task.FromResult(sMsg);
        }


        /// <summary>
        /// 回调接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("interface/callbackexternalcontact")]
        //[Consumes("text/xml")]
        public async Task CallbackExternalContact([FromQuery] CallbackRequest request)
        {
            string sToken = "x7XaZ";
            string sCorpID = "ww3f6a2088ec08814d";
            string sEncodingAESKey = "Na6hxrvfsRXlFo2TjA4alMXzYVXhar6BLlqsNj9Bl87";

            var body = Request.Body;
            var stream = new StreamReader(body);
            string postData = await stream.ReadToEndAsync();

            var options = new WechatWorkClientOptions()
            {
                CorpId = sCorpID,
                PushToken = sToken,
                PushEncodingAESKey = sEncodingAESKey,
            };
            var client = new WechatWorkClient(options);

            var callbackModel = client.DeserializeEventFromXml(postData);

            if (callbackModel.MessageType == "event" && callbackModel.FromUserName == "sys")
            {
                switch (callbackModel.Event)
                {
                    case "msgaudit_notify": //产生会话回调事件
                        await _easyRedisClient.AddAsync("wxwork:msgaudit_notify", 1);
                        break;
                    case "change_external_contact": //企业客户事件
                        var changeExternalContactEvent = client.DeserializeEventFromXml<ChangeExternalContactEvent>(postData);
                        await _mediator.Send(new ChangeExternalContactCommand(changeExternalContactEvent), HttpContext.RequestAborted);
                        break;
                }
            }
        }
        


        /// <summary>
        /// 定时任务调用处理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("interface/handlenotify")]
        public async Task<string> HandleNotify()
        {
            //await _mediator.Send(new GetChatDataCommand(), HttpContext.RequestAborted);
            string key = "wxwork:msgaudit_notify";
            string handlekey = "wxwork:msgaudit_notify_handle";

            long nowTime = DateTime.Now.AddMinutes(-2).D2ISecond();
            if (await _easyRedisClient.ExistsAsync(key) && !await _easyRedisClient.ExistsAsync(handlekey))
            {
                await _mediator.Send(new GetChatDataCommand(), HttpContext.RequestAborted);

                await _easyRedisClient.RemoveAsync(handlekey, StackExchange.Redis.CommandFlags.FireAndForget);
                await _easyRedisClient.RemoveAsync(key, StackExchange.Redis.CommandFlags.FireAndForget);
            }
            return await Task.FromResult("sccuess");
        }

       
        /// <summary>
        /// 初始化客户群和群群成员
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("interface/init")]
        public async Task<string> Init()
        {
            await _mediator.Send(new GroupchatInitCommand(), HttpContext.RequestAborted);
            return await Task.FromResult("sccuess");
        }
    }
}
