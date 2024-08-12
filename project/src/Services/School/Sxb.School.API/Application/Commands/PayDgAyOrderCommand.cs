using MediatR;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using System;

namespace Sxb.School.API.Application.Commands
{
    public class PayDgAyOrderCommand:IRequest
    {
        public Guid OrderId { get; set; }

        public DgAyOrderPayWay PayWay { get; set; }
    }
}
