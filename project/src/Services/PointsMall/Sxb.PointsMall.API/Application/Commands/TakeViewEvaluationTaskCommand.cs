using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using System;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class TakeViewEvaluationTaskCommand : IRequest<ResponseResult>
    {
        public Guid UserId { get; set; }
        public DateTime StartTime { get; set; }
    }
}
