using System;

namespace Sxb.SignActivity.API.Application.IntegrationEvents
{
    public class WeChatMsgIntegrationEvent
    {
        public Guid UserId { get; set; }

        public string Msg { get; set; }

        /// <summary>
        /// 模板第一个信息
        /// </summary>
        public string TemplateFirst { get; set; } = "签到成功";

        /// <summary>
        /// 失败尝试发送短信
        /// </summary>
        public bool FailTrySMS { get; set; } = true;

        /// <summary>
        /// 跳转钱包
        /// </summary>
        public bool IsBlink { get; set; } = true;
    }
}