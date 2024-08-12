using MediatR;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.API.Infrastructure.Services;
using Sxb.PointsMall.Domain.AggregatesModel.UserSignInInfoAggregate;
using Sxb.PointsMall.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.PointsMall.API.Application.Commands
{
    public class SignInNotifyCommandHandler : IRequestHandler<SignInNotifyCommand, ResponseResult>
    {
        private readonly IMediator _mediator;
        private readonly IUserSignInInfoRepository _userSignInInfoRepository;
        private readonly IUserService _userService;

        public SignInNotifyCommandHandler(IUserSignInInfoRepository userSignInInfoRepository, IMediator mediator, IUserService userService)
        {
            _userSignInInfoRepository = userSignInInfoRepository;
            _mediator = mediator;
            _userService = userService;
        }

        public async Task<ResponseResult> Handle(SignInNotifyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //开启需要先关注公众号
                if (request.Enabled)
                {
                    var isSubscribe = await _userService.GetUserSubscribe(request.UserId);
                    if (!isSubscribe)
                    {
                        return ResponseResult.Failed("请先关注公众号");
                    }
                }

                var userSignInInfo = await _userSignInInfoRepository.FindFromAsync(request.UserId);
                if (userSignInInfo == null)
                {
                    userSignInInfo = new UserSignInInfo(request.UserId);
                    await _userSignInInfoRepository.AddAsync(userSignInInfo);
                }


                //修改关注状态
                var fields = userSignInInfo.SetNotify(request.Enabled);
                var succeed = await _userSignInInfoRepository.UpdateAsync(userSignInInfo, fields);
                if (succeed)
                {
                    return ResponseResult.Success();
                }
                return ResponseResult.Failed("更新失败");
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }
    }
}
