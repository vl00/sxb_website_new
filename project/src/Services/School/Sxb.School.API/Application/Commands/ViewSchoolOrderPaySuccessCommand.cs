using MediatR;
using Sxb.School.Domain.AggregateModels.ViewOrder;
using System;

namespace Sxb.School.API.Application.Commands
{
    public class ViewSchoolOrderPaySuccessCommand : IRequest
    {

        public Guid OrderId { get; set; }

        public ViewOrderPayWay PayWay { get; set; }

    }
}
