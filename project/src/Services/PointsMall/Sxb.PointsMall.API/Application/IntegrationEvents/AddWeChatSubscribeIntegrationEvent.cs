using System;

namespace Sxb.PointsMall.API.Application.IntegrationEvents
{
    public class AddWeChatSubscribeIntegrationEvent
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id => UserId;

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 微信openId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 关注时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
