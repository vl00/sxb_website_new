using MediatR;
using System;

namespace Sxb.School.API.Application.Commands
{
    public class OpenViewPermissionCommand:IRequest<bool>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public  Guid  UserId{ get; set; }


        /// <summary>
        /// 学部ID
        /// </summary>
        public Guid ExtId { get; set; }
    }
}
