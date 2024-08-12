using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sxb.PointsMall.Domain.Events;
using Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;

namespace Sxb.PointsMall.API.Application.DomainEventHandlers
{
    /// <summary>
    /// 当用户完成积分任务时，加积分处理。
    /// </summary>
    public class AddAccountPointsWhenTaskCompleteDomainEventHandler : INotificationHandler<GrantTaskDomainEvent>
    {
        IAccountPointsItemRepository _accountPointsItemRepository;
        IAccountPointsRepository _accountPointsRepository;
        public AddAccountPointsWhenTaskCompleteDomainEventHandler(IAccountPointsItemRepository accountPointsItemRepository, IAccountPointsRepository accountPointsRepository)
        {
            _accountPointsItemRepository = accountPointsItemRepository;
            _accountPointsRepository = accountPointsRepository;
        }

        public async Task Handle(GrantTaskDomainEvent notification, CancellationToken cancellationToken)
        {


            var userPointsTask = notification.UserPointsTask;
            var accountPoints = await _accountPointsRepository.FindFromAsync(notification.UserPointsTask.UserId);
            if (accountPoints == null)
            {
                accountPoints = new AccountPoints(notification.UserPointsTask.UserId);
                await _accountPointsRepository.AddAsync(accountPoints);
            }
            accountPoints.AddPoints(userPointsTask.GetPoints, GetAccountPointsOriginType(userPointsTask.PointsTask.Type), userPointsTask.Id.ToString(), userPointsTask.PointsTask.Name);
            bool updateFlag = await _accountPointsRepository.UpdateAsync(accountPoints, nameof(accountPoints.Points), nameof(accountPoints.ModifyTime));
            if (!updateFlag) throw new Exception("当用户完成积分任务时加积分失败。原因：更新账户积分时失败。");
            foreach (var accountPointsItem in accountPoints.AccountPointsItems)
            {
                await _accountPointsItemRepository.AddAsync(accountPointsItem);
            }

        }


        public static AccountPointsOriginType GetAccountPointsOriginType(PointsTaskType pointsTaskType)
        {
            if (pointsTaskType == PointsTaskType.DayTask)
                return AccountPointsOriginType.DayTask;
            if (pointsTaskType == PointsTaskType.OperationTask)
                return AccountPointsOriginType.OperationTask;
            return 0;
        }
    }
}
