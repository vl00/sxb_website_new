using MediatR;
using System;

namespace Sxb.School.API.Application.Commands
{
    public class UpdateViewSchoolOrderCommand:IRequest
    {
        public Guid OrderId { get; set; }

        public decimal Amount { get; set; }
    }
}
