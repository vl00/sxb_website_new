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
    /// 加账户积分
    /// </summary>
    public class AddAccountPointsCommandHandler : IRequestHandler<AddAccountPointsCommand>
    {
        IAccountPointsItemRepository _accountPointsItemRepository;
        IAccountPointsRepository _accountPointsRepository;
        public AddAccountPointsCommandHandler(IAccountPointsItemRepository accountPointsItemRepository
            , IAccountPointsRepository accountPointsRepository)
        {
            _accountPointsItemRepository = accountPointsItemRepository;
            _accountPointsRepository = accountPointsRepository;
        }


        public async Task<Unit> Handle(AddAccountPointsCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Remark)) throw new ArgumentNullException("请在Remark中进行简述加分原因。");
            var accountPoints = await _accountPointsRepository.FindFromAsync(request.UserId);
            if (accountPoints == null)
            {
                accountPoints = new AccountPoints(request.UserId);
                await _accountPointsRepository.AddAsync(accountPoints);
            }
            accountPoints.AddPoints(request.Points, request.OriginType, request.OriginId, request.Remark);
            try
            {
                _accountPointsRepository.UnitOfWork.BeginTransaction();
                await _accountPointsRepository.UpdateAsync(accountPoints, nameof(accountPoints.Points), nameof(accountPoints.ModifyTime));
                foreach (var accountPointsItem in accountPoints.AccountPointsItems)
                {
                    await _accountPointsItemRepository.AddAsync(accountPointsItem);
                }
                _accountPointsRepository.UnitOfWork.CommitTransaction();
            }
            catch (Exception ex)
            {
                _accountPointsRepository.UnitOfWork.RollBackTransaction();
                throw new Exception("加账户积分失败。", ex);
            }
            return Unit.Value;
        }
    }
}
