using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.API.Infrastructure.DistributedLock;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using Sxb.PointsMall.Domain.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class CreateAndFinishTaskCommandHandler : IRequestHandler<CreateAndFinishTaskCommand, ResponseResult>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateAndFinishTaskCommandHandler> _logger;
        private readonly IUserPointsTaskRepository _userPointsTaskRepository;

        IDistributedLockFactory _distributedLockFactory;
        public CreateAndFinishTaskCommandHandler(IMediator mediator
            , ILogger<CreateAndFinishTaskCommandHandler> logger
            , IUserPointsTaskRepository userPointsTaskRepository
            , IDistributedLockFactory distributedLockFactory)
        {
            _mediator = mediator;
            _logger = logger;
            _userPointsTaskRepository = userPointsTaskRepository;
            _distributedLockFactory = distributedLockFactory;
        }

        public async Task<ResponseResult> Handle(CreateAndFinishTaskCommand request, CancellationToken cancellationToken)
        {
            //lock ...
            await using var distributedLock = _distributedLockFactory.CreateRedisDistributedLock();
            var  lockTakeFlag = await distributedLock.LockTakeAndWaitAsync($"CreateAndFinishTaskCommand:{request.UserId}", TimeSpan.FromSeconds(30), 30);
            if (!lockTakeFlag)
            {
                throw new Exception("系统繁忙，请稍后重试。");
            }

            if (request.UsingTransaction)
            {
                try
                {
                    _userPointsTaskRepository.UnitOfWork.BeginTransaction();

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
            else
            {
                return await MainAsync(request);
            }
        }

        /// <summary>
        /// 1.同一任务必须先把未完成任务完成后， 才能继续下一个
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseResult> MainAsync(CreateAndFinishTaskCommand request)
        {
            var userId = request.UserId;
            var taskId = request.TaskId;
            var fromId = request.FromId;
            var remark = request.Remark;
            var finishTime = request.FinishTime;
            var signInDays = request.SignInDays;
            //允许种草前日23：59：59创建，次日完成
            if (!(finishTime >= DateTime.Now.AddDays(-1)))
            {
                throw new Exception("完成时间错误，不能超过一天");
            }

            //1.获取当日未完成的任务
            var userPointsTask = await _userPointsTaskRepository.FindFromAsync(userId, taskId, status: UserPointsTaskStatus.UnFinish, finishTime);
            var (todayTimes, totalTimes) = await _userPointsTaskRepository.FindCountAsync(userId, taskId, finishTime);

            //无则创建任务
            if (userPointsTask == null)
            {
                //不能创建超出日限、总限次数的任务
                userPointsTask = await CreateAsync(userId, taskId, todayTimes, totalTimes, remark, signInDays);
            }

            //完成任务
            var fields = await userPointsTask.FinishAsync(finishTime, fromId);
            var succeed = await _userPointsTaskRepository.UpdateAsync(userPointsTask, fields);

            //实时发放积分
            //todo 以后再说
            if (taskId == PointsTask.SignInTaskId)
            {
                fields = await userPointsTask.GrantAsync(_mediator);
                await _userPointsTaskRepository.UpdateAsync(userPointsTask, fields);
            }

            if (succeed)
            {
                return ResponseResult.Success();
            }
            throw new Exception("更新失败");
        }



        public async Task<UserPointsTask> CreateAsync(Guid userId, int taskId, int todayTimes, int totalTimes, string remark, int signInDays)
        {
            var task = await _userPointsTaskRepository.FindTaskFromAsync(taskId);

            var getPoints = task.GetPoints(signInDays);
            var userPointsTask = new UserPointsTask(Guid.NewGuid()
                , userId
                , task
                , getPoints: getPoints
                , DateTime.Now
                , endFinishTime: null);

            userPointsTask.Remark = remark;

            userPointsTask.CheckTimes(todayTimes, totalTimes);

            var succeed = await _userPointsTaskRepository.AddAsync(userPointsTask);
            if (succeed)
                return userPointsTask;
            throw new Exception("创建任务失败");
        }
    }
}
