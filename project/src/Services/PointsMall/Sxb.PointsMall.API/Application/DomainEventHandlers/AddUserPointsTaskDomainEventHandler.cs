using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sxb.PointsMall.Domain.Events;
using Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using Sxb.PointsMall.API.Application.Commands;
using Sxb.Framework.Foundation;
using Microsoft.Extensions.Logging;

namespace Sxb.PointsMall.API.Application.DomainEventHandlers
{
    /// <summary>
    /// 当用户完成积分任务时，加积分处理。
    /// </summary>
    public class AddUserPointsTaskDomainEventHandler : INotificationHandler<AddUserPointsTaskDomainEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AddUserPointsTaskDomainEventHandler> _logger;

        public AddUserPointsTaskDomainEventHandler(IMediator mediator, ILogger<AddUserPointsTaskDomainEventHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(AddUserPointsTaskDomainEvent evt, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateAndFinishTaskCommand()
            {
                UserId = evt.UserId,
                TaskId = PointsTask.SignInTaskId,
                FromId = evt.Id.ToString(),
                Remark = evt.Remark,
                FinishTime = evt.FinishTime,
                UsingTransaction = false,
                SignInDays = evt.SignInDays,
            });
            if (!result.Succeed)
            {
                //logger
                _logger.LogError("领域事件添加积分任务错误,evt={0},result={1}", evt.ToJson(), result.ToJson());
                throw new Exception("领域事件添加积分任务错误:" + result.Msg);
            }
        }
    }
}
