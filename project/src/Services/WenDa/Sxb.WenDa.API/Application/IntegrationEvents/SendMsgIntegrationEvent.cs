using System;
using System.Collections.Generic;

namespace Sxb.WenDa.API.Application.IntegrationEvents
{
    public class SendMsgIntegrationEvent
    {
        private SendMsgIntegrationEvent(string templateSettingCode, Dictionary<string, string> extraData)
        {
            TemplateSettingCode = templateSettingCode ?? throw new ArgumentNullException(nameof(templateSettingCode));
            ExtraData = extraData;
        }

        public SendMsgIntegrationEvent(string templateSettingCode, string openId, Dictionary<string, string> extraData) : this(templateSettingCode, extraData)
        {
            OpenId = openId;
        }

        public string TemplateSettingCode { get; set; }

        public Guid? UserId { get; set; }

        public string OpenId { get; set; }

        public Dictionary<string, string> ExtraData { get; set; }
    }
}
