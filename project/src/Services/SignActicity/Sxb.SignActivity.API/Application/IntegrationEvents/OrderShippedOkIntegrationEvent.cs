using System;

namespace Sxb.SignActivity.API.Application.IntegrationEvents
{
    public class OrderShippedOkIntegrationEvent
    {
        public Guid OrderId { get; set; }

        public Guid UserId { get; set; }

    }
}