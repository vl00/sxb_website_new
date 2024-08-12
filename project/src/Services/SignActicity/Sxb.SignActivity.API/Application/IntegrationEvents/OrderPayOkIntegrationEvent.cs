using System;

namespace Sxb.SignActivity.API.Application.IntegrationEvents
{
    public class OrderPayOkIntegrationEvent
    {
        public Guid OrderId { get; set; }

        public Guid UserId { get; set; }

    }
}