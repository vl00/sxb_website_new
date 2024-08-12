using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class FinishTaskCommandHandler : IRequestHandler<FinishTaskCommand, ResponseResult>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FinishTaskCommandHandler> _logger;
        private readonly IUserPointsTaskRepository _userPointsTaskRepository;

        public FinishTaskCommandHandler(IMediator mediator, ILogger<FinishTaskCommandHandler> logger, IUserPointsTaskRepository userPointsTaskRepository)
        {
            _mediator = mediator;
            _logger = logger;
            _userPointsTaskRepository = userPointsTaskRepository;
        }

        public Task<ResponseResult> Handle(FinishTaskCommand request, CancellationToken cancellationToken)
        {
            //lock
            return MainAsync(request);
        }

        public async Task<ResponseResult> MainAsync(FinishTaskCommand request)
        {
            var userId = request.UserId;
            var taskId = request.TaskId;
            var fromId = request.FromId;
            var finishTime = DateTime.Now;


            //1.获取当日的任务有无完成
            var userPointsTask = await _userPointsTaskRepository.FindFromAsync(userId
                , taskId, UserPointsTaskStatus.UnFinish, finishTime);

            if (userPointsTask == null)
            {
                return ResponseResult.Failed("今日未创建任务");
            }

            var (todayTimes, totalTimes) = await _userPointsTaskRepository.FindCountAsync(userId, taskId, finishTime);
            userPointsTask.CheckTimes(todayTimes, totalTimes);
            var fields = await userPointsTask.FinishAsync(finishTime, todayTimes + 1, fromId);

            //todo 以后再说
            if (userPointsTask.PointsTask.Id == 1)
            {
                await userPointsTask.GrantAsync(_mediator);
            }

            var succeed = await _userPointsTaskRepository.UpdateAsync(userPointsTask, fields);

            if (succeed)
                return ResponseResult.Success();
            return ResponseResult.Failed("更新失败");
        }
    }
}
