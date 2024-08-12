using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System;

namespace Sxb.SignActivity.API.Application.Commands
{
    public class SignInParentCommand : IRequest<ResponseResult>
    {
        public Guid UserId { get; set; }
    }
}
