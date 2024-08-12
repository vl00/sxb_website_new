using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotNetCore.CAP;
using EasyWeChat.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.Framework.Foundation;
using Sxb.SignActivity.API.Application.Commands;

namespace Sxb.SignActivity.API.Application.IntegrationEvents
{


    public class OrderShippedIntegrationEventHandler : ICapSubscribe, IOrderShippedIntegrationEventHandler
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderShippedIntegrationEventHandler> _logger;

        public OrderShippedIntegrationEventHandler(IMediator mediator, ILogger<OrderShippedIntegrationEventHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [CapSubscribe(nameof(OrderShippedOkIntegrationEvent))]
        public async Task Handle(OrderShippedOkIntegrationEvent evt)
        {
            if (evt == null)
            {
                _logger.LogError("接收的数据为空");
                return;
            }

            _logger.LogInformation("data=" + evt.ToJson());
            var result = await _mediator.Send(new OrderShippedCommand() { UserId = evt.UserId, OrderId = evt.OrderId });
            if (!result.Succeed)
            {

            }
        }
    }
}