using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.Framework.Foundation;
using Sxb.PointsMall.API.Application.Commands;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;

namespace Sxb.PointsMall.API.Application.IntegrationEvents
{

    public class AddUserPointsTaskIntegrationEventHandler : ICapSubscribe, IAddUserPointsTaskIntegrationEventHandler
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AddUserPointsTaskIntegrationEventHandler> _logger;

        public AddUserPointsTaskIntegrationEventHandler(IMediator mediator, ILogger<AddUserPointsTaskIntegrationEventHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [CapSubscribe(nameof(AddEvaluationIntegrationEvent))]
        public async Task Handle(AddEvaluationIntegrationEvent evt)
        {
            if (evt == null)
            {
                _logger.LogError("AddEvaluation,接收的数据为空");
                return;
            }
            _logger.LogInformation("AddEvaluation,data={0}", evt.ToJson());


            var result = await _mediator.Send(new CreateAndFinishTaskCommand()
            {
                UserId = evt.UserId,
                TaskId = PointsTask.AddEvaluatinTaskId,
                FromId = evt.Id.ToString(),
                Remark = evt.Title,
                FinishTime = evt.CreateTime
            });
            _logger.LogInformation("result={0}", result.ToJson());
            if (!result.Succeed)
            {
                //logger
            }
        }


        [CapSubscribe(nameof(AddChildIntegrationEvent))]
        public async Task Handle(AddChildIntegrationEvent evt)
        {
            if (evt == null)
            {
                _logger.LogError("AddChild,接收的数据为空");
                return;
            }
            _logger.LogInformation("AddChild,data={0}", evt.ToJson());


            var result = await _mediator.Send(new CreateAndFinishTaskCommand()
            {
                UserId = evt.UserId,
                TaskId = PointsTask.AddChildTaskId,
                FromId = evt.Id.ToString(),
                Remark = evt.Name,
                FinishTime = evt.CreateTime
            });
            _logger.LogInformation("result={0}", result.ToJson());
            if (!result.Succeed)
            {
                //logger
            }
        }

#if DEBUG
        [CapSubscribe(nameof(OrdersPayOkIntegrationEvent), Group = "Local1")]
#else
        [CapSubscribe(nameof(OrdersPayOkIntegrationEvent))]
#endif
        public async Task Handle(OrdersPayOkIntegrationEvent evt)
        {
            if (evt == null)
            {
                _logger.LogError("OrdersPayOk,接收的数据为空");
                return;
            }
            _logger.LogInformation("OrdersPayOk,data={0}", evt.ToJson());

            if (evt.AdvanceOrderIsPointsPay)
            {
                _logger.LogError("OrdersPayOk,积分支付不能完成任务");
                return;
            }
            try
            {
                var command = new CreateAndFinishTaskCommand()
                {
                    UserId = evt.UserId,
                    TaskId = PointsTask.OrderTaskId,
                    FromId = evt.AdvanceOrderId.ToString(),
                    Remark = evt.GetProductNames(128),
                    FinishTime = evt.PaymentTime
                };

                var result = await _mediator.Send(command);
                _logger.LogInformation("result={0}", result.ToJson());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrdersPayOk");
                throw;
            }
        }


        [CapSubscribe(nameof(AddWeChatSubscribeIntegrationEvent))]
        public async Task Handle(AddWeChatSubscribeIntegrationEvent evt)
        {
            if (evt == null)
            {
                _logger.LogError("AddWeChatSubscribe,接收的数据为空");
                return;
            }
            _logger.LogInformation("AddWeChatSubscribe,data={0}", evt.ToJson());


            var result = await _mediator.Send(new CreateAndFinishTaskCommand()
            {
                UserId = evt.UserId,
                TaskId = PointsTask.WeChatSubcribeTaskId,
                FromId = evt.Id.ToString(),
                Remark = evt.OpenId,
                FinishTime = evt.CreateTime
            });
            _logger.LogInformation("result={0}", result.ToJson());
            if (!result.Succeed)
            {
                //logger
            }
        }
    }
}