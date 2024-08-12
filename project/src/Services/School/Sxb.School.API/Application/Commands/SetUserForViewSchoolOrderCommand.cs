namespace Sxb.School.API.Application.Commands
{
    using MediatR;
    using Sxb.School.Domain.AggregateModels.ViewOrder;
    using System;

    public class SetUserForViewSchoolOrderCommand:IRequest<SchoolViewOrder>
    {
        public Guid OrderId { get; set; }

        public Guid UserId { get; set; }
    }
}
