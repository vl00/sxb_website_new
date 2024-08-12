using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.API.Application.DomainEventHandlers;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using Sxb.PointsMall.Domain.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class GrantTaskCommandHandler : IRequestHandler<GrantTaskCommand, ResponseResult>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GrantTaskCommandHandler> _logger;
        private readonly IUserPointsTaskRepository _userPointsTaskRepository;

        public GrantTaskCommandHandler(IMediator mediator, ILogger<GrantTaskCommandHandler> logger, IUserPointsTaskRepository userPointsTaskRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _userPointsTaskRepository = userPointsTaskRepository;
        }

        public async Task<ResponseResult> Handle(GrantTaskCommand request, CancellationToken cancellationToken)
        {
            //lock ...
            try
            {
                _userPointsTaskRepository.UnitOfWork.BeginTransaction();

                //仅能完成一次任务
                var resp = await MainAsync(request);

                _userPointsTaskRepository.UnitOfWork.CommitTransaction();
                return resp;
            }
            catch (Exception ex)
            {
                _userPointsTaskRepository.UnitOfWork.RollBackTransaction();
                return ResponseResult.Failed(ex.Message);
            }
        }

        public async Task<ResponseResult> MainAsync(GrantTaskCommand request)
        {
            var userId = request.UserId;
            var taskId = request.TaskId;
            var today = DateTime.Today.Date;

            var userPointsTask = await _userPointsTaskRepository.FindFromAsync(userId, taskId, status: UserPointsTaskStatus.Finished, taskDate: null);
            if (userPointsTask == null)
            {
                return ResponseResult.Failed("无待领取任务");
            }

            //日常任务必须当日领取
            //运营任务随时可以领取
            if (userPointsTask.PointsTask.Type == PointsTaskType.DayTask && userPointsTask.CreateTime.Date != today)
            {
                return ResponseResult.Failed("无待领取任务");
            }

            //修改任务为已完成
            var fields = await userPointsTask.GrantAsync(_mediator);
            var succeed = await _userPointsTaskRepository.UpdateAsync(userPointsTask, fields);

            if (succeed)
            {
                return ResponseResult.Success();
            }
            throw new Exception("更新失败");
        }
    }
}
