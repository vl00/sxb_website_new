using MediatR;
using System;

namespace Sxb.School.API.Application.Commands
{
    public class SetDgAyOrderUserCommand : IRequest<bool>
    {

        public Guid OrderId { get; set; }

        public Guid UserId { get; set; }
    }
}
