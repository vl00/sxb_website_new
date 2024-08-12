using Sxb.Domain;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Sxb.Infrastructure.Core.Extensions
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, EFContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, Entity entiy)
        {

            if (entiy.DomainEvents?.Any() == true)
            {
                foreach (var domainEvent in entiy.DomainEvents)
                    await mediator.Publish(domainEvent);
                entiy.ClearDomainEvents();
            }


        }
        public static  async Task DispatchDomainEventsAsync(this IMediator mediator, IEnumerable<Entity> entiys)
        {

            List<IDomainEvent> domainEvents = new List<IDomainEvent>();
            entiys.ToList().ForEach(entity => {
                if (entity.DomainEvents?.Any() == true)
                {
                    domainEvents.AddRange(entity.DomainEvents);
                }
                entity.ClearDomainEvents();
            });

            if (domainEvents?.Any() == true)
            {
                foreach (var domainEvent in domainEvents)
                {
                    await mediator.Publish(domainEvent);

                }
            }

        }

    }
}
