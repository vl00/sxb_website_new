using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.User.API.Application.IntegrationEvents
{
    public class TodoSomethingIntegrationEvent
    {
        public TodoSomethingIntegrationEvent(Guid userId) => UserId = userId;
        public Guid UserId { get; }
    }
}
