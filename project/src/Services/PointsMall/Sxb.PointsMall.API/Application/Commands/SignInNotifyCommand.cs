using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class SignInNotifyCommand : IRequest<ResponseResult>
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// 是否启用提醒
        /// </summary>
        public bool Enabled { get; set; }
    }
}
