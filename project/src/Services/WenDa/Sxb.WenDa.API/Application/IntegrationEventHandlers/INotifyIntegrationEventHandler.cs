using DotNetCore.CAP;
using Sxb.WenDa.API.Application.IntegrationEvents;

namespace Sxb.WenDa.API.Application.IntegrationEventHandlers
{
    public interface INotifyIntegrationEventHandler
    {

        [CapSubscribe("NotifyIntegrationEvent", false)]
        Task SendAsync(NotifyIntegrationEvent @event);
    }
}