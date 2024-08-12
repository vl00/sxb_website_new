using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using System;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class GrantTaskCommand : IRequest<ResponseResult>
    {
        public Guid UserId { get; set; }
        public int TaskId { get; set; }
    }
}
