using MediatR;
using Sxb.School.Domain.AggregateModels.DgAyOrderAggregate;
using System;

namespace Sxb.School.API.Application.Commands
{
    public class SetDgAyOrderStatusCommand:IRequest
    {
        public Guid OrderId { get; set; }
        public DgAyOrderState State { get; set; }
    }
}
