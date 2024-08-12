using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using System;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class ViewEvaluationTaskCommand : IRequest<ResponseResult>
    {
        public Guid UserId { get; set; }
        public PointsTask PointsTask { get; set; }
    }
}
