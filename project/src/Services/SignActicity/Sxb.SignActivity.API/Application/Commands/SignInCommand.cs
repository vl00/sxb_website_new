using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System;

namespace Sxb.SignActivity.API.Application.Commands
{
    public class SignInCommand : IRequest<ResponseResult>
    {
        public Guid UserId { get; set; }

        public DateTime SignInDate { get; set; } = DateTime.Today.Date;
    }
}
