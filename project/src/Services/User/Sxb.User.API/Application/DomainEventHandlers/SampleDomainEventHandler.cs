using DotNetCore.CAP;
using Sxb.Domain;
using Sxb.User.API.Application.IntegrationEvents;
using Sxb.User.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.User.API.Application.DomainEventHandlers
{
    public class SampleDomainEventHandler : IDomainEventHandler<SampleDomainEvent>
    {
        ICapPublisher _capPublisher;
        public SampleDomainEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task Handle(SampleDomainEvent notification, CancellationToken cancellationToken)
        {
            //发送一个集成事件
            await _capPublisher.PublishAsync("TodoSomething", new TodoSomethingIntegrationEvent(notification.Talent.UserId));
        }
    }
}
