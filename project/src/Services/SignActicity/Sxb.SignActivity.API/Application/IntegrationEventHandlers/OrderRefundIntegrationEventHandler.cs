using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MediatR;

namespace Sxb.SignActivity.API.Application.IntegrationEvents
{


    public class OrderRefundIntegrationEventHandler : ICapSubscribe, IOrderRefundIntegrationEventHandler
    {
        private readonly IMediator _mediator;

        public OrderRefundIntegrationEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }


        [CapSubscribe("OrderRefundSuccessIntegrationEvent")]
        public Task Handle(OrderRefundSuccessIntegrationEvent evt)
        {
            Console.WriteLine(evt.RefundUserId);
            return Task.CompletedTask;
        }
    }
}