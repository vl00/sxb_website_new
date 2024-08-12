using DotNetCore.CAP;
using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.Framework.Cache.Redis;
using Sxb.SignActivity.API.Application.IntegrationEvents;
using Sxb.SignActivity.Common.Entity;
using Sxb.SignActivity.Common.Enum;
using Sxb.SignActivity.Infrastructure.Repositories;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using Sxb.SignActivity.Common.OtherAPIClient.Marketing;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter.Model;
using System.Text;
using Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter;
using Sxb.SignActivity.Common.DTO;

namespace Sxb.SignActivity.API.Application.Commands
{
    public class RecoveryShippedCommandHandle : IRequestHandler<RecoveryShippedCommand, object>
    {
        private readonly ILogger<RecoveryShippedCommand> _logger;
        private readonly ICapPublisher _capPublisher;
        private readonly IMediator _mediator;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IMarketingAPIClient _marketingAPIClient;

        private readonly SignActivity.Query.SQL.IRepository.ISignInRepository _signInRepository;
        private readonly SignActivity.Query.SQL.IRepository.ISignInHistoryRepository _signInHistoryRepository;
        private readonly SignActivity.Query.SQL.IRepository.ISignInParentHistoryRepository _signInParentHistoryRepository;
        private readonly SignActivity.Query.SQL.IRepository.IOrderRepository _orderRepository;

        public RecoveryShippedCommandHandle(ILogger<RecoveryShippedCommand> logger, ICapPublisher capPublisher, IMediator mediator, SignActivity.Query.SQL.IRepository.ISignInRepository signInRepository, SignActivity.Query.SQL.IRepository.IOrderRepository orderRepository, SignActivity.Query.SQL.IRepository.ISignInHistoryRepository signInHistoryRepository, IEasyRedisClient easyRedisClient, SignActivity.Query.SQL.IRepository.ISignInParentHistoryRepository signInParentHistoryRepository, IMarketingAPIClient marketingAPIClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _capPublisher = capPublisher ?? throw new ArgumentNullException(nameof(capPublisher));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _signInRepository = signInRepository ?? throw new ArgumentNullException(nameof(signInRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _signInHistoryRepository = signInHistoryRepository;
            _easyRedisClient = easyRedisClient;
            _signInParentHistoryRepository = signInParentHistoryRepository;
            _marketingAPIClient = marketingAPIClient;
        }

        public bool IsSend { get; set; }

        public async Task<object> Handle(RecoveryShippedCommand request, CancellationToken cancellationToken)
        {
            IsSend = request.IsSend;
            var buNo = BuNoType.SHUANG11_ACTIVITY.ToString();
            DateTime signInDate = request.SignInDate ?? DateTime.Today;
            return await RecoveryAsync(buNo, signInDate);
        }

        private async Task SendRequest(Guid OrderId, Guid orderUserId)
        {
            if (!IsSend)
            {
                return;
            }

            //1.直接调用
            var data = await _mediator.Send(new OrderShippedCommand() { UserId = orderUserId, OrderId = OrderId });
            if (!data.Succeed)
            {
                _logger.LogError("调用失败" + data.ToJson());
            }

            //2.使用消息
            //await _capPublisher.PublishAsync(nameof(OrderPayOkIntegrationEvent), new OrderPayOkIntegrationEvent()
            //{
            //    OrderId = orderId,
            //    UserId = orderUserId,
            //});
        }

        private async Task<object> RecoveryAsync(string buNo, DateTime signInDate)
        {
            var orders = await _orderRepository.GetListAsync(new DateTime(2021, 11, 1), new DateTime(2021, 11, 22), status: 333);
            orders = orders.Distinct(new OrderPaymentTimeComparer());

            //签到记录
            var signInHistories = await _signInHistoryRepository.GetListAsync(buNo);
            List<Order> errors = new List<Order>();
            foreach (var order in orders)
            {
                //ExistsUnblockHistoryAsync
                var exists = signInHistories.Any(s => s.MemberId == order.Userid && s.Blocked == false && s.UnblockSignInDate == signInDate.Date);
                //此单, 未解冻过
                if (!exists)
                {
                    errors.Add(order);
                    await SendRequest(order.Id, order.Userid);
                    break;
                }
            }
            return errors.Select(s=> new 
            {
                s.Id,
                s.Userid,
                s.Payment,
                s.Paymenttime,
                s.Status,
                s.ModifyDateTime
            });
        }
    }
}
