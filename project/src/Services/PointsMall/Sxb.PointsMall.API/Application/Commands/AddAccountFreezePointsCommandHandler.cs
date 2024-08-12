using MediatR;
using Sxb.PointsMall.Domain.AggregatesModel.PointsAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{

    /// <summary>
    /// 加冻结积分
    /// </summary>
    public class AddAccountFreezePointsCommandHandler : IRequestHandler<AddAccountFreezePointsCommand, Guid>
    {
        IAccountPointsItemRepository _accountPointsItemRepository;
        IAccountPointsRepository _accountPointsRepository;
        public AddAccountFreezePointsCommandHandler(IAccountPointsItemRepository accountPointsItemRepository
            , IAccountPointsRepository accountPointsRepository)
        {
            _accountPointsItemRepository = accountPointsItemRepository;
            _accountPointsRepository = accountPointsRepository;
        }


        public async Task<Guid> Handle(AddAccountFreezePointsCommand request, CancellationToken cancellationToken)
        {
            var accountPoints = await _accountPointsRepository.FindFromAsync(request.UserId);
            if (accountPoints == null)
            {
                accountPoints = new AccountPoints(request.UserId);
                await _accountPointsRepository.AddAsync(accountPoints);
            }
            try
            {
                _accountPointsRepository.UnitOfWork.BeginTransaction();
                Guid freezeId = accountPoints.AddFreezePoints(request.FreezePoints, request.OriginType, request.OriginId, request.Remark);
                await _accountPointsRepository.UpdateAsync(accountPoints, nameof(accountPoints.FreezePoints), nameof(accountPoints.ModifyTime));
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
                throw new Exception("增加冻结积分失败。", ex);
            }
        }
    }
}
