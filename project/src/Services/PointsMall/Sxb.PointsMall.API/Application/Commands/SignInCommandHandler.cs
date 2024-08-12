using MediatR;
using Sxb.PointsMall.API.Infrastructure.DistributedLock;
using Sxb.PointsMall.Domain.AggregatesModel.UserSignInInfoAggregate;
using Sxb.PointsMall.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class SignInCommandHandler : IRequestHandler<SignInCommand, bool>
    {
        private readonly IMediator _mediator;
        IUserSignInInfoRepository _userSignInInfoRepository;
        IDistributedLockFactory _distributedLockFactory;

        public SignInCommandHandler(IUserSignInInfoRepository userSignInInfoRepository, IMediator mediator, IDistributedLockFactory distributedLockFactory)
        {
            _userSignInInfoRepository = userSignInInfoRepository;
            _mediator = mediator;
            _distributedLockFactory = distributedLockFactory;
        }

        public async Task<bool> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            await using var distributedLock = _distributedLockFactory.CreateRedisDistributedLock();
            var lockTakeFlag = await distributedLock.LockTakeAndWaitAsync($"SignInCommand:{request.UserId}", TimeSpan.FromSeconds(30), 30);
            if (!lockTakeFlag)
            {
                throw new Exception("系统繁忙，请稍后重试。");
            }
            var userSignInInfo = await _userSignInInfoRepository.FindFromAsync(request.UserId);
            if (userSignInInfo == null)
            {
                userSignInInfo = new UserSignInInfo(request.UserId);
                await _userSignInInfoRepository.AddAsync(userSignInInfo);
            }
            userSignInInfo.DaySignIn();
            await _mediator.Publish(new AddUserPointsTaskDomainEvent()
            {
                Id = userSignInInfo.Id,
                UserId = userSignInInfo.UserId,
                FinishTime = userSignInInfo.LastSignDate.GetValueOrDefault(),
                Remark = $"第{userSignInInfo.SignInDays}天签到奖励",
                SignInDays = userSignInInfo.SignInDays
            });
            return await _userSignInInfoRepository.UpdateContinueDaysAsync(userSignInInfo);

        }
    }
}
