using System;

namespace Sxb.SignActivity.API.Application.IntegrationEvents
{
    public class OrderRefundSuccessIntegrationEvent
    {
        public Guid OrderId { get; set; }

        public Guid OrderDetailId { get; set; }

        public Guid RefundUserId { get; set; }

    }
}