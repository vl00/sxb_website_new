using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using EasyWeChat.CustomMessage;
using EasyWeChat.Interface;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using Sxb.Framework.Cache.Redis;
using Sxb.SignActivity.Common.OtherAPIClient.Marketing;
using Sxb.Framework.Foundation;
using EasyWeChat.Model;
using Sxb.SignActivity.Common.OtherAPIClient.Marketing.Model;
using Microsoft.Extensions.Options;
using Sxb.SignActivity.Common.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Crypto;

namespace Sxb.SignActivity.API.Application.IntegrationEvents
{
    public class WeChatMsgIntegrationEventHandler : ICapSubscribe, IWeChatMsgIntegrationEventHandler
    {
        private readonly ICustomMessageService _customMessageService;
        private readonly ITemplateMessageService _templateMessageService;
        private readonly IEasyRedisClient _easyRedisClient;

        private readonly IWeChatAPIClient _weChatAPIClient;
        private readonly IMarketingAPIClient _marketingAPIClient;
        private readonly IOperationAPIClient _operationAPIClient;
        private readonly ILogger<WeChatMsgIntegrationEventHandler> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly WeChatConfig _weChatConfig;

        public WeChatMsgIntegrationEventHandler(ICustomMessageService customMessageService, ITemplateMessageService templateMessageService, IEasyRedisClient easyRedisClient,
            IWeChatAPIClient weChatAPIClient, IMarketingAPIClient marketingAPIClient, ILogger<WeChatMsgIntegrationEventHandler> logger, IOperationAPIClient operationAPIClient,
            IOptions<WeChatConfig> options, IWebHostEnvironment webHostEnvironment)
        {
            _customMessageService = customMessageService ?? throw new ArgumentNullException(nameof(customMessageService));
            _templateMessageService = templateMessageService ?? throw new ArgumentNullException(nameof(templateMessageService));
            _easyRedisClient = easyRedisClient ?? throw new ArgumentNullException(nameof(easyRedisClient));
            _weChatAPIClient = weChatAPIClient ?? throw new ArgumentNullException(nameof(weChatAPIClient));
            _marketingAPIClient = marketingAPIClient ?? throw new ArgumentNullException(nameof(marketingAPIClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _operationAPIClient = operationAPIClient;

            _weChatConfig = options.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        bool enableSendMsg = true;

        public string Format(string msg, UserWxOpenInfo openInfo)
        {
            return msg.Replace("{userName}", openInfo.UserName)
                .Replace("{userPhone}", openInfo.UserPhone)
                ;
        }

        [CapSubscribe(nameof(WeChatMsgIntegrationEvent))]
        public async Task Handle(WeChatMsgIntegrationEvent evt)
        {
            _logger.LogError("发送消息" + evt.ToJson());
            if (!enableSendMsg)
            {
                _logger.LogError("发送消息失败, 未开启发送消息");
                return;
            }

            var openInfo = await _marketingAPIClient.GetWxOpenInfo(evt.UserId);
            if (openInfo == null)
            {
                _logger.LogError("发送微信消息失败, 无User");
                return;
            }

            _logger.LogError("发送消息, openInfo={0}", openInfo.ToJson());
            var msg = Format(evt.Msg, openInfo);
            var first = evt.TemplateFirst;
            var isBlink = evt.IsBlink;

            if (evt.FailTrySMS)
            {
                await SendTryAsync(openInfo, first, msg, isBlink);
            }
            else
            {
                await SendWxTemplateMsgAsync(openInfo, first, msg, isBlink);
            }
        }

        public async Task SendWxTemplateMsgAsync(UserWxOpenInfo openInfo, string first, string msg, bool isBlink)
        {
            if (string.IsNullOrWhiteSpace(openInfo.OpenId))
            {
                _logger.LogError("发送消息失败, openId为空");
                return;
            }

            string toUser = openInfo.OpenId;
            var fwhAccessToken = await _weChatAPIClient.GetAccessToken();
            //微信模板消息
            await SendWxTemplateMsg(first, msg, isBlink, toUser, fwhAccessToken);
        }

        public async Task SendTryAsync(UserWxOpenInfo openInfo, string first, string msg, bool isBlink)
        {
            if (string.IsNullOrWhiteSpace(openInfo.OpenId) && string.IsNullOrWhiteSpace(openInfo.UserPhone))
            {
                _logger.LogError("发送消息失败, openId与phone均为空");
                return;
            }

            //优先使用微信消息, 否则发送短信息
            if (!string.IsNullOrWhiteSpace(openInfo.OpenId))
            {
                string toUser = openInfo.OpenId;
                var fwhAccessToken = await _weChatAPIClient.GetAccessToken();

                //发送微信消息
                //var succeed = await SeneWxMsg(msg, toUser, fwhAccessToken);
                var succeed = false;

                //尝试使用微信模板消息
                if (!succeed)
                {
                    succeed = await SendWxTemplateMsg(first, msg, isBlink, toUser, fwhAccessToken);
                }
                //尝试使用短信息
                if (!succeed)
                {
                    await SendPhone(msg, openInfo.UserPhone);
                }
            }
            else
            {
                await SendPhone(msg, openInfo.UserPhone);
            }
        }

        private async Task<bool> SeneWxMsg(string msg, string toUser, string fwhAccessToken)
        {
            try
            {
                var message = new TextMessage();
                message.SetContent(msg);
                var result = await _customMessageService.SendAsync(fwhAccessToken, toUser, message);

                if (!result.IsSuccess)
                {
                    _logger.LogError("发送微信消息失败,result=" + result.ToJson());
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送微信消息失败" + ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 发送短信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        private async Task<bool> SendPhone(string msg, string phone)
        {
            try
            {
                if (phone == null)
                {
                    return false;
                }
                if (!phone.StartsWith("86"))
                {
                    phone = "86" + phone;
                }

                //您好{1}，感谢您使用上学帮！
                var smsMsgResult = await _operationAPIClient.SMSApi("798721", new[] { phone }, new string[] { msg });

                if (smsMsgResult.statu != 1
                    || smsMsgResult.sendStatus.Length == 0
                    || smsMsgResult.sendStatus[0].code == null
                    || smsMsgResult.sendStatus[0].code.ToLower() != "ok")
                {
                    _logger.LogError("发送手机消息失败,result=" + smsMsgResult.ToJson());
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送手机消息失败" + ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 发送微信模板消息
        /// </summary>
        /// <param name="first"></param>
        /// <param name="msg"></param>
        /// <param name="toUser"></param>
        /// <param name="fwhAccessToken"></param>
        /// <returns></returns>
        private async Task<bool> SendWxTemplateMsg(string first, string msg, bool isBlink, string toUser, string fwhAccessToken)
        {
            try
            {
                var templateConfig = _weChatConfig.MsgTemplates.SignIn;

                //发送不成功，尝试使用模板消息再推一次
                var templateMsg = new SendTemplateRequest(toUser, templateConfig.TemplateId);
                var fields = new List<TemplateDataFiled> {
                    new TemplateDataFiled {Filed = "first", Value = first},
                    new TemplateDataFiled {Filed = "keyword1", Value = msg },
                    new TemplateDataFiled {Filed = "keyword2", Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                };

                if (isBlink)
                {
                    fields.Add(new TemplateDataFiled { Filed = "remark", Value = "点击详情查看我的钱包" });

                    if (_webHostEnvironment.IsProduction())
                    {
                        string appID = _weChatConfig.MiniProgramOrg.AppID;
                        templateMsg.SetMiniprogram(appID, templateConfig.PagePath);
                    }
                    else
                    {
                        //测试
                        templateMsg.Url = "https://user3.sxkid.com/my/wallet.html";
                    }
                }

                templateMsg.SetData(fields.ToArray());
                var tempMsgResult = await _templateMessageService.SendAsync(fwhAccessToken, templateMsg);


                if (tempMsgResult.errcode != ResponseCodeEnum.success)
                {
                    _logger.LogError("发送微信模板消息失败,result=" + tempMsgResult.ToJson());
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送微信模板消息失败" + ex.Message);
            }
            return true;
        }
    }
}