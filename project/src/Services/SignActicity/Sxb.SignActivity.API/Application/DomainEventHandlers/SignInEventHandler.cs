using Sxb.Domain;
using Sxb.SignActivity.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.SignActivity.API.Application.DomainEventHandlers
{
    public class SignInEventHandler : IDomainEventHandler<NewSignDomainEvent>
    {
        public Task Handle(NewSignDomainEvent notification, CancellationToken cancellationToken)
        {
               

            return Task.CompletedTask;
        }
    }
}
