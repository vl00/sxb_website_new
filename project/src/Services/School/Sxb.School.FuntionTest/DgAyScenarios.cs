using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxb.School.API.Application.DomainEventHandlers;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using Sxb.School.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.FuntionTest
{
    [TestClass]
    public class DgAyScenarios : SchoolScenariosBase
    {

        [TestMethod]
        public async Task TriggerPaySuccessEvent_Test()
        {
            IMediator mediator = application.Services.GetService<IMediator>();
            IDgAyOrderRepository repository = application.Services.GetService<IDgAyOrderRepository>();
            var order = await repository.GetAsync(Guid.Parse("3B0F1D50-D34B-4676-8C22-312D5A1361C7"));
            await mediator.Publish(new DgAyOrderPaySuccessDomainEvent() { Order = order });


        }
    }
}
