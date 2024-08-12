using MediatR;
using Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class DeductFreezePointsCommandHandler : IRequestHandler<DeductFreezePointsCommand>
    {

        IAccountPointsRepository _accountPointsRepository;
        IAccountPointsItemRepository _accountPointsItemRepository;

        public DeductFreezePointsCommandHandler(IAccountPointsItemRepository accountPointsItemRepository
            , IAccountPointsRepository accountPointsRepository)
        {
            _accountPointsItemRepository = accountPointsItemRepository;
            _accountPointsRepository = accountPointsRepository;
        }

        public async  Task<Unit> Handle(DeductFreezePointsCommand request, CancellationToken cancellationToken)
        {
            var freezeItem = await _accountPointsItemRepository.GetAsync(request.FreezeId);
            if (freezeItem == null) throw new KeyNotFoundException("找不到该明细，无法操作");
            if (freezeItem.UserId != request.UserId) throw new ArgumentException("用户ID无法匹配该冻结ID,无法操作");
            if (!freezeItem.IsFreeze) throw new ArgumentException("该单非冻结明细，无法操作");
            var accountPoints = await _accountPointsRepository.FindFromAsync(request.UserId); //积分账户
            if (accountPoints == null) throw new KeyNotFoundException("找不到当前账户的积分账户，无法操作");
            freezeItem.DeFreeze();
            accountPoints.DeductFreezePoints(freezeItem,request.OriginType);
            try
            {
                _accountPointsRepository.UnitOfWork.BeginTransaction();
                await _accountPointsItemRepository.UpdateAsync(freezeItem, nameof(freezeItem.State));
                await _accountPointsRepository.UpdateAsync(accountPoints, nameof(accountPoints.FreezePoints), nameof(accountPoints.ModifyTime));
                foreach (var accountPointsItem in accountPoints.AccountPointsItems)
                {
                    await _accountPointsItemRepository.AddAsync(accountPointsItem);
                }
                _accountPointsRepository.UnitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _accountPointsRepository.UnitOfWork.RollBackTransaction();
                throw new Exception($"扣除冻结积分失败。frezzeId = {freezeItem.Id}", ex);
            }
            return Unit.Value;

        }
    }
}
