using MediatR;
using Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate;
using Sxb.PointsMall.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class FreezePointsCommandHandler : IRequestHandler<FreezePointsCommand, Guid>
    {
        IAccountPointsRepository _accountPointsRepository;
        IAccountPointsItemRepository _accountPointsItemRepository;
        public FreezePointsCommandHandler(IAccountPointsRepository accountPointsRepository
            , IAccountPointsItemRepository accountPointsItemRepository)
        {
            _accountPointsRepository = accountPointsRepository;
            _accountPointsItemRepository = accountPointsItemRepository;
        }

        public async Task<Guid> Handle(FreezePointsCommand request, CancellationToken cancellationToken)
        {
            var accountPoints = await _accountPointsRepository.FindFromAsync(request.UserId);
            if (accountPoints == null) throw new KeyNotFoundException("找不到当前账户的积分账户，无法操作");
            try
            {
                _accountPointsRepository.UnitOfWork.BeginTransaction();
                Guid freezeId = accountPoints.PointsToFreezePoints(request.FreezePoints, request.OriginType, request.OriginId, request.Remark);
                await _accountPointsRepository.UpdateAsync(accountPoints, nameof(accountPoints.FreezePoints), nameof(accountPoints.Points), nameof(accountPoints.ModifyTime));
                foreach (var accountPointsItem in accountPoints.AccountPointsItems)
                {
                    await _accountPointsItemRepository.AddAsync(accountPointsItem);
                }
                _accountPointsRepository.UnitOfWork.CommitTransaction();
                return freezeId;
            }
            catch (Exception ex)
            {
                _accountPointsRepository.UnitOfWork.RollBackTransaction();
                throw new Exception("冻结积分失败。", ex);
            }
        }
    }
}
