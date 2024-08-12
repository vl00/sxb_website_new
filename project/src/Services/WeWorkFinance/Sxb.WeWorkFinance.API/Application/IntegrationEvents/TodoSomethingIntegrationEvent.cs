using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.IntegrationEvents
{
    public class TodoSomethingIntegrationEvent
    {
        public TodoSomethingIntegrationEvent(string userId) => UserId = userId;
        public string UserId { get; }
    }
}
