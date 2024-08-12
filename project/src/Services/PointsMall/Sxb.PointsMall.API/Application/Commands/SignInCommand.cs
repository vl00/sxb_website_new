using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class SignInCommand:IRequest<bool>
    {
        public Guid UserId { get; set; }
    }
}
