using DotNetCore.CAP;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.User.API.Application.IntegrationEvents
{
    /// <summary>
    /// 集成事件订阅
    /// </summary>
    public class SubscriberService : ISubscriberService, ICapSubscribe
    {
        public SubscriberService()
        {
        }

        [CapSubscribe("TodoSomething")]
        public void TodoSomething(TodoSomethingIntegrationEvent @event)
        {
            var aaa = @event.UserId;
            //TodoSomething
        }
    }
}
