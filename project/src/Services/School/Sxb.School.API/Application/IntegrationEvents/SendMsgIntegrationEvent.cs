using System;
using System.Collections.Generic;

namespace Sxb.School.API.Application.IntegrationEvents
{
    public class SendMsgIntegrationEvent
    {
        public string TemplateSettingCode { get; set; }

        public Guid? UserId { get; set; }

        public string OpenId { get; set; }

        public Dictionary<string, string> ExtraData { get; set; }

        public SendMsgIntegrationEvent()
        {
        }

        public SendMsgIntegrationEvent(string templateSettingCode, Guid? userId, Dictionary<string, string> extraData)
        {
            TemplateSettingCode = templateSettingCode ?? throw new ArgumentNullException(nameof(templateSettingCode));
            UserId = userId;
            ExtraData = extraData ?? throw new ArgumentNullException(nameof(extraData));
        }
    }
}
