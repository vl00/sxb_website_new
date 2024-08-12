using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.Framework.Foundation;
using Sxb.SignActivity.API.Application.Commands;

namespace Sxb.SignActivity.API.Application.IntegrationEvents
{


    public class OrderPayIntegrationEventHandler : ICapSubscribe, IOrderPayIntegrationEventHandler
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderPayIntegrationEventHandler> _logger;
        private readonly SignActivity.Query.SQL.IRepository.IOrderRepository _orderRepository;

        public OrderPayIntegrationEventHandler(IMediator mediator, ILogger<OrderPayIntegrationEventHandler> logger, SignActivity.Query.SQL.IRepository.IOrderRepository orderRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _orderRepository = orderRepository;
        }

        [CapSubscribe(nameof(OrderPayOkIntegrationEvent))]
        public async Task Handle(OrderPayOkIntegrationEvent evt)
        {
            if (evt == null)
            {
                _logger.LogError("接收的数据为空");
                return;
            }

            _logger.LogInformation("data=" + evt.ToJson());


            var signInDate = DateTime.Today.Date;
            try
            {
                signInDate = await GetSignInDate(evt.OrderId, evt.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            var result = await _mediator.Send(new SignInCommand() { UserId = evt.UserId, SignInDate = signInDate });
            if (!result.Succeed)
            {
                //logger
            }
        }


        private async Task<DateTime> GetSignInDate(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetAsync(orderId);
            if (order == null || order.Paymenttime == null)
            {
                throw new ArgumentException(nameof(order), $"此单不存在或未支付,order={order?.ToJson()}");
            }

            if (userId != order.Userid)
            {
                throw new ArgumentException($"OrderId与UserId不匹配");
            }

            //签到日期
            return order.Paymenttime.GetValueOrDefault().Date;
        }
    }
}