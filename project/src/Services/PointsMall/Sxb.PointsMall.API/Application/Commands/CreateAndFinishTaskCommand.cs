using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using System;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class CreateAndFinishTaskCommand : IRequest<ResponseResult>
    {
        public Guid UserId { get; set; }
        public int TaskId { get; set; }
        public string FromId { get; set; }
        public string Remark { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime FinishTime { get; set; } = DateTime.Now;


        public int SignInDays { get; set; }


        public bool UsingTransaction { get; set; } = true;
    }
}
