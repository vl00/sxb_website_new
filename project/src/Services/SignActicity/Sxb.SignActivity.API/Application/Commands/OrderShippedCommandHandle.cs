using DotNetCore.CAP;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductManagement.Framework.Foundation;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.SignActivity.API.Application.IntegrationEvents;
using Sxb.SignActivity.Common.Enum;
using Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter;
using Sxb.SignActivity.Common.OtherAPIClient.FinanceCenter.Model;
using Sxb.SignActivity.Common.OtherAPIClient.Marketing;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using Sxb.SignActivity.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.SignActivity.API.Application.Commands
{
    public class OrderShippedCommandHandle : IRequestHandler<OrderShippedCommand, ResponseResult>
    {
        private readonly ILogger<OrderShippedCommandHandle> _logger;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ICapPublisher _capPublisher;

        private readonly ISignInRepository _signInRepository;
        private readonly ISignInHistoryRepository _signInHistoryRepository;
        private readonly ISignInParentHistoryRepository _signInParentHistoryRepository;
        private readonly SignActivity.Query.SQL.IRepository.ISignConfigRepository _signConfigRepository;
        private readonly SignActivity.Query.SQL.IRepository.IOrderRepository _orderRepository;

        private readonly IFinanceCenterAPIClient _financeCenterAPIClient;
        private readonly IMarketingAPIClient _marketingAPIClient;

        public OrderShippedCommandHandle(ILogger<OrderShippedCommandHandle> logger, IEasyRedisClient easyRedisClient, ISignInRepository signInRepository, ISignInHistoryRepository signInHistoryRepository, SignActivity.Query.SQL.IRepository.ISignConfigRepository signConfigRepository, SignActivity.Query.SQL.IRepository.IOrderRepository orderRepository, IFinanceCenterAPIClient financeCenterAPIClient, IMarketingAPIClient marketingAPIClient, ICapPublisher capPublisher, ISignInParentHistoryRepository signInParentHistoryRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _easyRedisClient = easyRedisClient ?? throw new ArgumentNullException(nameof(easyRedisClient));
            _signInRepository = signInRepository ?? throw new ArgumentNullException(nameof(signInRepository));
            _signInHistoryRepository = signInHistoryRepository ?? throw new ArgumentNullException(nameof(signInHistoryRepository));
            _signConfigRepository = signConfigRepository ?? throw new ArgumentNullException(nameof(signConfigRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _financeCenterAPIClient = financeCenterAPIClient ?? throw new ArgumentNullException(nameof(financeCenterAPIClient));
            _marketingAPIClient = marketingAPIClient ?? throw new ArgumentNullException(nameof(marketingAPIClient));
            _capPublisher = capPublisher;
            _signInParentHistoryRepository = signInParentHistoryRepository;
        }

        public async Task<ResponseResult> Handle(OrderShippedCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogError("确认收货数据为空");
            }
            _logger.LogInformation($"开始确认收货data={request.ToJson()},now={DateTime.Now}");

            //request.UserId = Guid.Parse("{D6580810-C79C-40D4-8B84-F62D663DFFE1}");

            Guid userId = request.UserId;
            if (userId == Guid.Empty)
            {
                return ResponseResult.Failed("用户Id为空");
            }

            //lock(userId)
            var key = string.Format(RedisKeys.SignInLockKey, userId);
            var locked = await _easyRedisClient.LockTakeAsync(key, userId.ToString(), TimeSpan.FromSeconds(3));
            if (locked)
            {
                try
                {
                    ResponseResult result;
                    if (request.OrderId != Guid.Empty)
                    {
                        var order = await _orderRepository.GetAsync(request.OrderId);
                        if (order == null || order.Paymenttime == null)
                        {
                            result = ResponseResult.Failed($"此单不存在或未支付,order={order?.ToJson()}");
                        }
                        else if(userId != order.Userid)
                        {
                            result = ResponseResult.Failed("OrderId与UserId不匹配");
                        }
                        else
                        {
                            request.SignInDate = order.Paymenttime.Value.Date;
                            result = await Main(request, cancellationToken);
                        }
                    }
                    else if (request.SignInDate != null)
                    {
                        result = await Main(request, cancellationToken);
                    }
                    else
                    {
                        result = ResponseResult.Failed("OrderId与SignInDate至少选择一个");
                    }


                    if (!result.Succeed)
                    {
                        _logger.LogError("解冻失败:{0}", result.Msg);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    //return ResponseResult.Failed(ex.Message);
                    throw;
                }
                finally
                {
                    //release lock
                    await _easyRedisClient.LockReleaseAsync(key, userId.ToString());
                }
            }
            else
            {
                _logger.LogError("操作频繁, 请稍后再试");
                //抛错, 让mq重试
                throw new AbandonedMutexException("操作频繁, 请稍后再试");
            }
        }

        public async Task<ResponseResult> Main(OrderShippedCommand request, CancellationToken cancellationToken)
        {
            //最大参与人数
            var buNo = BuNoType.SHUANG11_ACTIVITY.ToString();
            Guid userId = request.UserId;
            DateTime signInDate = request.SignInDate.GetValueOrDefault();

            //check activity
            var signConfig = await _signConfigRepository.GetAsync(buNo);
            if (signConfig == null) { return ResponseResult.Failed("活动不存在"); }

            if (!(signInDate >= signConfig.StartTime && signInDate <= signConfig.EndTime))
            {
                return ResponseResult.Failed("非活动期间的单");
            }

            var exists = await _signInHistoryRepository.ExistsUnblockHistoryAsync(buNo, userId, signInDate);
            if (exists)
            {
                return ResponseResult.Failed($"已解冻日期,signInDate={signInDate}");
            }

            //记录打卡统计和打卡记录
            var signIn = await _signInRepository.GetUserSignAsync(buNo, userId);
            if (signIn == null)
            {
                return ResponseResult.Failed("此用户未参与打卡");
            }

            //从前往后, 实际解冻的签到日期, 从第一次签到开始
            var signInHistory = await _signInHistoryRepository.GetFirstBlockHistoryAsync(buNo, userId);
            if (signInHistory == null)
            {
                return ResponseResult.Failed("此用户已解冻所有打卡金额");
            }

            //打卡成功的最小金额
            decimal signAmountLimit = await _easyRedisClient.GetOrAddAsync(RedisKeys.SignInAmountLimitKey, () => SignConstantValue.SignAmount);
            //当日已收货金额
            var totalOrderAmount = await _orderRepository.GetShippedTotalAmountAsync(userId, signInDate);
            if (request.UnCheckSignAmount || totalOrderAmount >= signAmountLimit)
            {
                _logger.LogInformation($"确认收货,解冻开始");

                signIn.UnBlock();
                await _signInRepository.UpdateAsync(signIn, cancellationToken);

                //解冻自己
                await UnBlock(signInHistory, signInDate, cancellationToken);

                //解冻上级
                if (signIn.BindParentId != null)
                {
                    //父级邀请奖励记录
                    var signInParentHistory = await _signInParentHistoryRepository.GetFirstBlockParentHistoryAsync(buNo, userId);
                    await UnBlockParent(signInParentHistory, signIn.BindParentUnfreezeInviteCount, cancellationToken);
                }

                _logger.LogInformation($"确认收货,解冻结束");
                return ResponseResult.Success();
            }
            else
            {
                //当日收货总金额低于45
                return ResponseResult.Failed($"确认收货,解冻失败,但当日收货总金额低于{signAmountLimit}");
            }
        }


        /// <summary>
        /// 解冻自己
        /// </summary>
        /// <param name="signInHistory"></param>
        /// <param name="unblockSignInDate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task UnBlock(SignInHistory signInHistory, DateTime unblockSignInDate, CancellationToken cancellationToken)
        {
            var userId = signInHistory.MemberId.GetValueOrDefault();

            //修改数据库
            signInHistory.UnBlock(modifier: userId, unblockSignInDate);
            await _signInHistoryRepository.UpdateAsync(signInHistory, cancellationToken);
            await _signInHistoryRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            //去支付中心解冻
            var resp = await UnFreezeMoney(signInHistory.Param3);
            if (!resp.Succeed)
            {
                _logger.LogError("UnBlock去支付中心解冻失败,resp={0}", resp.ToJson());
                //撤回
                //signInHistory.ReBlock(modifier: userId);
                //await _signInHistoryRepository.UpdateAsync(signInHistory, cancellationToken);
            }
            //发送解冻消息
            await SendMessageAsync(userId, signInHistory.SignCount, signInHistory.RewardMoney);
        }

        /// <summary>
        /// 解冻上级
        /// </summary>
        /// <param name="signInParentHistory"></param>
        /// <param name="bindParentUnfreezeInviteCount"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> UnBlockParent(SignInParentHistory signInParentHistory, int bindParentUnfreezeInviteCount, CancellationToken cancellationToken)
        {
            if (signInParentHistory != null && bindParentUnfreezeInviteCount >= signInParentHistory.ParentInviteCount)
            {
                //修改数据库
                signInParentHistory.UnBlock();
                await _signInParentHistoryRepository.UpdateAsync(signInParentHistory, cancellationToken);
                await _signInParentHistoryRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                //去支付中心解冻
                var resp = await UnFreezeMoney(signInParentHistory.Param3);
                if (!resp.Succeed)
                {
                    _logger.LogError("UnBlockParent去支付中心解冻失败,resp={0}", resp.ToJson());
                    //撤回
                    //signInParentHistory.ReBlock();
                    //await _signInParentHistoryRepository.UpdateAsync(signInParentHistory, cancellationToken);
                }
                //发送解冻消息
                await SendParentMessageAsync(signInParentHistory.MemberId, signInParentHistory.ParentId, signInParentHistory.ParentInviteCount, signInParentHistory.ParentRewardMoney);
            }
            return true;
        }

        public async Task<APIResult<string>> UnFreezeMoney(string freezeMoneyInLogId)
        {
            var resp = await _financeCenterAPIClient.InsideUnFreezeAmount(freezeMoneyInLogId);

            if (!resp.Succeed)
            {
                _logger.LogError("解冻签到奖励失败, " + resp.ToJson());
            }
            return resp;
        }


        public async Task SendParentMessageAsync(Guid userId, Guid parentUserId, int inviteCount, int reward)
        {
            var user = await _marketingAPIClient.GetWxOpenInfo(userId);
            var userName = user?.UserName ?? "";

            var msg = $"您累计的邀请{userName}打卡{inviteCount}次奖励已成功到账{reward}元";
            await _capPublisher.PublishAsync(nameof(WeChatMsgIntegrationEvent), new WeChatMsgIntegrationEvent()
            {
                UserId = parentUserId,
                Msg = msg
            });
        }
        public async Task SendMessageAsync(Guid userId, int signCount, int reward)
        {
            var msg = $"您参与的第{signCount}次打卡活动已成功到账{reward}元";
            await _capPublisher.PublishAsync(nameof(WeChatMsgIntegrationEvent), new WeChatMsgIntegrationEvent()
            {
                UserId = userId,
                Msg = msg
            });
        }
    }
}
