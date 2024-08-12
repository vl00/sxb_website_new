using DotNetCore.CAP;
using MediatR;
using Microsoft.Extensions.Logging;
using Sxb.Framework.Cache.Redis;
using Sxb.SignActivity.API.Application.IntegrationEvents;
using Sxb.SignActivity.Common.Entity;
using Sxb.SignActivity.Common.Enum;
using Sxb.SignActivity.Infrastructure.Repositories;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using Sxb.SignActivity.Common.OtherAPIClient.Marketing;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter.Model;
using System.Text;
using Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter;

namespace Sxb.SignActivity.API.Application.Commands
{
    public class RecoverySignInCommandHandle : IRequestHandler<RecoverySignInCommand, object>
    {
        private readonly ILogger<SignInCommandHandle> _logger;
        private readonly ICapPublisher _capPublisher;
        private readonly IMediator _mediator;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IMarketingAPIClient _marketingAPIClient;

        private readonly SignActivity.Query.SQL.IRepository.ISignInRepository _signInRepository;
        private readonly SignActivity.Query.SQL.IRepository.ISignInHistoryRepository _signInHistoryRepository;
        private readonly SignActivity.Query.SQL.IRepository.ISignInParentHistoryRepository _signInParentHistoryRepository;
        private readonly SignActivity.Query.SQL.IRepository.IOrderRepository _orderRepository;

        public RecoverySignInCommandHandle(ILogger<SignInCommandHandle> logger, ICapPublisher capPublisher, IMediator mediator, SignActivity.Query.SQL.IRepository.ISignInRepository signInRepository, SignActivity.Query.SQL.IRepository.IOrderRepository orderRepository, SignActivity.Query.SQL.IRepository.ISignInHistoryRepository signInHistoryRepository, IEasyRedisClient easyRedisClient, SignActivity.Query.SQL.IRepository.ISignInParentHistoryRepository signInParentHistoryRepository, IMarketingAPIClient marketingAPIClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _capPublisher = capPublisher ?? throw new ArgumentNullException(nameof(capPublisher));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _signInRepository = signInRepository ?? throw new ArgumentNullException(nameof(signInRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _signInHistoryRepository = signInHistoryRepository;
            _easyRedisClient = easyRedisClient;
            _signInParentHistoryRepository = signInParentHistoryRepository;
            _marketingAPIClient = marketingAPIClient;
        }

        public bool IsSend { get; set; }

        public async Task<object> Handle(RecoverySignInCommand request, CancellationToken cancellationToken)
        {
            IsSend = request.IsSend;
            var buNo = BuNoType.SHUANG11_ACTIVITY.ToString();
            if (request.Parent)
            {
                return await RecoveryParentAsync(buNo);
            }


            DateTime signInDate = request.SignInDate ?? DateTime.Today;
            IEnumerable<Common.DTO.OrderDayPayDTO> orders = await GetOrderPays(signInDate);

            //补发签到
            var signIns = await _signInRepository.GetSignInsAsync(buNo);
            var signUserIds = signIns.Select(s => s.MemberId).ToList();

            //补发签到记录
            var signInHistories = await _signInHistoryRepository.GetListAsync(buNo, signInDate);
            var signHistoryUserIds = signInHistories.Select(s => s.MemberId).ToList();

            //补发签到
            var signInSendUserIds = await RecoverySignIn(signInDate, signUserIds, orders);

            //补发签到记录 排除已补的签到
            var excludeSignIn = signHistoryUserIds.Where(s => !signInSendUserIds.Contains(s)).ToList();
            //补发签到记录
            var signInHistorySendUserIds = await RecoverySignIn(signInDate, excludeSignIn, orders);

            return new
            {
                buNo,
                signInDate,
                signInSendUserIds,
                signInHistorySendUserIds
            };
        }

        private async Task SendRequest(DateTime signInDate, Guid orderUserId)
        {
            if (!IsSend)
            {
                return;
            }

            //1.直接调用
            var data = await _mediator.Send(new SignInCommand() { UserId = orderUserId, SignInDate = signInDate });
            if (!data.Succeed)
            {
                _logger.LogError("调用失败" + data.ToJson());
            }

            //2.使用消息
            //await _capPublisher.PublishAsync(nameof(OrderPayOkIntegrationEvent), new OrderPayOkIntegrationEvent()
            //{
            //    OrderId = orderId,
            //    UserId = orderUserId,
            //});
        }

        private async Task<IEnumerable<Common.DTO.OrderDayPayDTO>> GetOrderPays(DateTime signInDate)
        {

            //var orders = await _orderRepository.GetListAsync(signInDate, signInDate.AddDays(1));
            //var orderUserIds = orders.Select(s => s.Userid).Distinct().ToList();

            //打卡成功的最小金额
            decimal signAmountLimit = await _easyRedisClient.GetOrAddAsync(RedisKeys.SignInAmountLimitKey, () => SignConstantValue.SignAmount);

            var orders = await _orderRepository.GetOrderDayPaysAsync(userId: null, signInDate, signInDate.AddDays(1));
            var news = orders.Where(s => s.RealPayment >= signAmountLimit).ToList();
            return news;
        }

        private async Task<List<Guid>> RecoverySignIn(DateTime signInDate, IEnumerable<Guid> signUserIds, IEnumerable<Common.DTO.OrderDayPayDTO> orders)
        {
            List<Guid> sendUserIds = new List<Guid>();
            foreach (var order in orders)
            {
                var exists = signUserIds.Any(s => s == order.UserId);
                if (!exists)
                {
                    sendUserIds.Add(order.UserId);
                    await SendRequest(signInDate, order.UserId);
                }
            }
            return sendUserIds;
        }

        private async Task<List<object>> RecoveryParentAsync(string buNo)
        {


            var signIns = await _signInRepository.GetSignInsAsync(buNo);
            var parents = await _signInParentHistoryRepository.GetListAsync(buNo);


            DateTime signInDate = new DateTime(2021, 11, 1);
            List<object> errors = new List<object>();
            foreach (var sign in signIns)
            {
                //if (sign.MemberId != Guid.Parse("{50B8860D-57AC-4CD1-9006-E00BA9B30A59}"))
                //{
                //    continue;
                //}
                if (sign.SignCount == sign.BindParentInviteCount)
                {
                    //正确数据
                    continue;
                }

                var (parentUserId, joinTime) = await GetParentUserIdAsync(sign.MemberId);

                //没有父级
                if (parentUserId == null || parentUserId == Guid.Empty)
                    continue;

                //有父级, 未绑定
                if (sign.BindParentId == null)
                {
                    _logger.LogError("有父级, 未绑定");
                    if (IsSend)
                    {
                        //重新绑定父级
                        await _mediator.Send(new SignInParentCommand() { UserId = sign.MemberId });
                    }

                    errors.Add(new
                    {
                        sign.MemberId,
                        sign.SignInDate,
                        sign.SignCount,
                        sign.BindParentId,
                        sign.BindParentInviteCount,
                        sign.BindParentTime
                    });
                    continue;
                }


                var signCreateTime = sign.CreateTime.Value;
                //var bindTime = sign.BindParentTime.Value;

                //先有父级
                if (joinTime <= signCreateTime)
                {
                    if (sign.SignCount == sign.BindParentInviteCount)
                    {
                        //正确数据
                        continue;
                    }
                    _logger.LogError("先有父级. 签到次数错误");
                    if (IsSend)
                    {
                        //补发绑定父级
                        await _mediator.Send(new SignInParentCommand() { UserId = sign.MemberId });
                    }
                }
                //后有父级
                else
                {
                    if (sign.SignCount > sign.BindParentInviteCount)
                    {
                        //正确数据
                        continue;
                    }
                    _logger.LogError("后有父级. 签到次数错误");
                }
                errors.Add(new
                {
                    sign.MemberId,
                    sign.SignInDate,
                    sign.SignCount,
                    sign.CreateTime,
                    sign.BindParentId,
                    sign.BindParentInviteCount,
                    sign.BindParentTime
                });
            }

            return errors;
        }


        public async Task<(Guid?, DateTime?)> GetParentUserIdAsync(Guid userId)
        {
            //被邀请的有效对象，从10月29号开始锁定的下线（粉丝或顾问）或之前预锁定的用户
            //有上级, 并且在活动时间邀请的
            var fxUser = await _marketingAPIClient.GetFxUser(userId);
            //无上级
            if (fxUser != null && fxUser.ParentUserId != null)
            {
                //非活动时间邀请的
                if (fxUser.JoinTime > SignConstantValue.InviteStartTime) // && fxUser.JoinTime <= endTime
                {
                    return (fxUser.ParentUserId, fxUser.JoinTime);
                }
            }
            else
            {
                var preLockFxUser = await _marketingAPIClient.GetPreLockFxUser(userId);
                //if (preLockFxUser.CreateTime)
                {
                    return (preLockFxUser?.ParentUserId, preLockFxUser.Createime);
                }
            }
            return (null, null);
        }

    }
}
