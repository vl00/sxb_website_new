using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using System;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class FinishTaskCommand : IRequest<ResponseResult>
    {
        public Guid UserId { get; set; }
        public int TaskId { get; set; }
        public string FromId { get; set; }
    }
}
