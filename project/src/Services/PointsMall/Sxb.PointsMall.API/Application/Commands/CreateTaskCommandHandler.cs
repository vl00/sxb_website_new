using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, ResponseResult>
    {
        private readonly ILogger<CreateTaskCommandHandler> _logger;
        private readonly IUserPointsTaskRepository _userPointsTaskRepository;

        public CreateTaskCommandHandler(ILogger<CreateTaskCommandHandler> logger, IUserPointsTaskRepository userPointsTaskRepository)
        {
            _logger = logger;
            _userPointsTaskRepository = userPointsTaskRepository;
        }

        public async Task<ResponseResult> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            //lock
            return await MainAsync(request);
        }

        public async Task<ResponseResult> MainAsync(CreateTaskCommand request)
        {
            var userId = request.UserId;
            var taskId = request.TaskId;
            var taskDate = DateTime.Today;

            //1.获取当日的任务
            var userPointsTask = await _userPointsTaskRepository.FindFromAsync(userId
                , taskId, UserPointsTaskStatus.UnFinish, taskDate);

            if (userPointsTask != null)
            {
                return ResponseResult.Failed("已创建任务");
            }

            var task = await _userPointsTaskRepository.FindTaskFromAsync(taskId);
            userPointsTask = new UserPointsTask(Guid.NewGuid()
                , userId
                , task
                , finishTimes: 0
                , getPoints: task.TimesPoints
                , DateTime.Now
                , endFinishTime: null);

            var (todayTimes, totalTimes) = await _userPointsTaskRepository.FindCountAsync(userId, taskId, taskDate);
            userPointsTask.CheckTimes(todayTimes, totalTimes);

            await _userPointsTaskRepository.AddAsync(userPointsTask);

            return ResponseResult.Success(userPointsTask);
        }
    }
}
