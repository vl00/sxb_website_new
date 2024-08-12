using DotNetCore.CAP;
using EasyWeChat.Model;
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
    public class SignInCommandHandle : IRequestHandler<SignInCommand, ResponseResult>
    {
        private readonly ILogger<SignInCommandHandle> _logger;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ICapPublisher _capPublisher;

        private readonly ISignInRepository _signInRepository;
        private readonly ISignInHistoryRepository _signInHistoryRepository;
        private readonly ISignInParentHistoryRepository _signInParentHistoryRepository;
        private readonly SignActivity.Query.SQL.IRepository.ISignConfigRepository _signConfigRepository;
        private readonly SignActivity.Query.SQL.IRepository.IOrderRepository _orderRepository;

        private readonly IFinanceCenterAPIClient _financeCenterAPIClient;
        private readonly IMarketingAPIClient _marketingAPIClient;

        public SignInCommandHandle(ILogger<SignInCommandHandle> logger, IEasyRedisClient easyRedisClient, ISignInRepository signInRepository, ISignInHistoryRepository signInHistoryRepository, SignActivity.Query.SQL.IRepository.ISignConfigRepository signConfigRepository, SignActivity.Query.SQL.IRepository.IOrderRepository orderRepository, IFinanceCenterAPIClient financeCenterAPIClient, IMarketingAPIClient marketingAPIClient, ICapPublisher capPublisher, ISignInParentHistoryRepository signInParentHistoryRepository)
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

        public async Task<ResponseResult> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogError("签到数据为空");
            }
            _logger.LogInformation($"开始签到data={request.ToJson()},now={DateTime.Now}");
            //request.UserId = Guid.Parse("{D6580810-C79C-40D4-8B84-F62D663DFFE1}");

            Guid userId = request.UserId;
            if (userId == Guid.Empty)
            {
                return ResponseResult.Failed("用户Id为空");
            }

            //lock(userId)
            //var key = string.Format(RedisKeys.SignInLockKey, userId);
            //lock all
            var key = string.Format(RedisKeys.SignInLockKey, "all");
            var locked = await _easyRedisClient.LockTakeAsync(key, userId.ToString(), TimeSpan.FromSeconds(3));
            if (locked)
            {
                try
                {
                    var result = await Main(request, cancellationToken);
                    if (!result.Succeed)
                    {
                        _logger.LogError("签到失败:{0}", result.Msg);
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
        public async Task<ResponseResult> Main(SignInCommand request, CancellationToken cancellationToken)
        {
            //最大参与人数
            var buNo = BuNoType.SHUANG11_ACTIVITY.ToString();
            Guid userId = request.UserId;
            DateTime signInDate = request.SignInDate;

            var signConfig = await _signConfigRepository.GetAsync(buNo);
            //check activity
            if (signConfig == null) { return ResponseResult.Failed("活动不存在"); }

            if (!CheckSignInDate(signConfig, signInDate)) { return ResponseResult.Failed("活动不在有效期"); }

            //当日订单总额超过45元, 打卡成功
            if (!await CheckOrderAsync(userId, signInDate)) { return ResponseResult.Failed("当日订单金额不足"); }


            //奖励列表
            int[] rewards = signConfig.RewardList.FromJson<int[]>();

            //记录打卡统计
            var signIn = await _signInRepository.GetUserSignAsync(buNo, userId);
            bool isNewSign = signIn == null;
            int signUserLimit = 0;
            if (isNewSign)
            {
                //签到满员
                signUserLimit = await GetSignUserLimtAsync(buNo);
                if (signUserLimit >= signConfig.UserLimit)
                {
                    return ResponseResult.Failed("签到满员");
                }

                signIn = new SignIn(userId, signConfig.Threshold, "", "");
                signIn.SignOnce(signInDate, rewards, userId);
                await _signInRepository.AddAsync(signIn, cancellationToken);
            }
            else if (signIn.SignInDate.GetValueOrDefault().Date.Equals(signInDate.Date))
            {
                //今日已签到
                return ResponseResult.Failed($"日期{signIn.SignInDate}已签到");
            }
            else
            {
                signIn.SignOnce(signInDate, rewards, userId);
                await _signInRepository.UpdateAsync(signIn, cancellationToken);
            }
            await _signInRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);


            var existsHistory = await _signInHistoryRepository.ExistsHistoryAsync(buNo, userId, signInDate);
            if (existsHistory)
            {
                //今日已签到
                return ResponseResult.Failed($"日期{signInDate}已签到,存在签到记录");
            }

            //记录打卡记录
            var signInHistory = signIn.CreateSignInHistory("");
            await _signInHistoryRepository.AddAsync(signInHistory);
            await _signInHistoryRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            //发放的奖励
            int reward = signIn.RewardMoney;

            //发放冻结奖励到钱包,  退货则不解冻奖励
            var grantResponse = await GrantMoney(userId, reward, remark: "发放签到奖励");
            signInHistory.Param3 = grantResponse.Data.FreezeMoneyInLogId;

            await _signInHistoryRepository.UpdateAsync(signInHistory);
            await _signInHistoryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            //发放上级奖励
            await DispatchParentReward(signIn, signConfig.StartTime, signConfig.EndTime);


            //发送签到信息
            await SendMessageAsync(signIn, rewards, signConfig.EndTime);

            //签到人数++
            if (isNewSign)
            {
                signUserLimit++;
                await _easyRedisClient.ReplaceAsync(RedisKeys.SignInUserCountKey, signUserLimit);
            }
            return ResponseResult.Success();
        }

        /// <summary>
        /// 打卡成功的人数
        /// </summary>
        /// <param name="buNo"></param>
        /// <returns></returns>
        public async Task<int> GetSignUserLimtAsync(string buNo)
        {
            //不走缓存
            //int signUserLimit = _signInRepository.GetSignUserCount(buNo);
            //await _easyRedisClient.AddAsync(RedisKeys.SignInUserCountKey, signUserLimit, TimeSpan.FromDays(1));

            int signUserLimit = await _easyRedisClient.GetOrAddAsync(RedisKeys.SignInUserCountKey, () =>
            {
                return _signInRepository.GetSignUserCount(buNo);
            }, TimeSpan.FromMinutes(60));
            return signUserLimit;
        }

        public bool CheckSignInDate(Common.Entity.SignConfig signConfig, DateTime signInDate)
        {
            if (signConfig == null)
                return false;

            if (signConfig.StartTime > signInDate)
            {
                //活动未开始
                return false;
            }

            if (signConfig.EndTime < signInDate)
            {
                //活动已结束
                return false;
            }

            return true;
        }

        public async Task<bool> CheckOrderAsync(Guid userId, DateTime signDate)
        {
            DateTime startTime = signDate.Date;
            DateTime endTime = startTime.AddDays(1);

            //打卡成功的最小金额
            decimal signAmountLimit = await _easyRedisClient.GetOrAddAsync(RedisKeys.SignInAmountLimitKey, () => SignConstantValue.SignAmount);
            //当日实际订单金额 = 总支付 - 总退款
            decimal totalOrderAmount = await _orderRepository.GetTotalAmountAsync(userId, startTime, endTime);

            //当日订单总额超过45元, 打卡成功
            var canSign = totalOrderAmount >= signAmountLimit;
            if (!canSign)
            {
                _logger.LogError($"签到限制金额:{signAmountLimit},订单金额:{totalOrderAmount},日期:{signDate}");
            }
            return canSign;
        }


        public async Task<Guid?> GetParentUserIdAsync(Guid userId)
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
                    return fxUser.ParentUserId;
                }
            }
            else
            {
                var preLockFxUser = await _marketingAPIClient.GetPreLockFxUser(userId);
                //if (preLockFxUser.CreateTime)
                {
                    return preLockFxUser?.ParentUserId;
                }
            }
            return null;
        }

        /// <summary>
        /// 发放上级奖励
        /// 
        /// 1.已邀请
        /// 2.预锁粉
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DispatchParentReward(SignIn signIn, DateTime startTime, DateTime endTime)
        {
            var userId = signIn.MemberId;
            Guid parentUserId = signIn.BindParentId.GetValueOrDefault();
            //未绑定过父级
            //每次都验证绑定关系
            //if (parentUserId == Guid.Empty)
            {
                var lockParentUserId = await GetParentUserIdAsync(userId);
                if (lockParentUserId == null)
                {
                    return false;
                }
                parentUserId = lockParentUserId.Value;

                //绑定父级
                signIn.BindParent(parentUserId);
            }
            if (userId == parentUserId)
            {
                _logger.LogError($"邀请签到失败, 不能邀请自己, userId={userId}");
                return false;
            }

            int[] parentRewards = GetParentRewards();

            //修改SignIn表
            var (currentRewardMoney, nextRewardMoney) = signIn.BindParentInviteCountIncrease(parentRewards);
            await _signInRepository.UpdateAsync(signIn);
            await _signInRepository.UnitOfWork.SaveEntitiesAsync();

            //本次为父级带来的奖励
            if (currentRewardMoney > 0)
            {
                int parentInviteCount = signIn.BindParentInviteCount;
                int parentTotalMoney = signIn.BindParentTotalMoney;

                //创建ParentHistory
                var signInParentHistory = signIn.CreateSignInParentHistory(currentRewardMoney, parentTotalMoney);
                await _signInParentHistoryRepository.AddAsync(signInParentHistory);
                await _signInParentHistoryRepository.UnitOfWork.SaveEntitiesAsync();

                //保存发放冻结奖励id到ParentHistory
                var grantResponse = await GrantMoney(parentUserId, currentRewardMoney, remark: "发放邀请签到奖励");
                signInParentHistory.Param3 = grantResponse.Data.FreezeMoneyInLogId;
                await _signInParentHistoryRepository.UpdateAsync(signInParentHistory);

                if (grantResponse.Succeed)
                {
                    await SendParentMessageAsync(userId, parentUserId, parentRewards, parentInviteCount, parentTotalMoney, endTime);
                }
                else
                {
                    _logger.LogError($"邀请签到失败, 发放邀请签到奖励失败, data={signIn.ToJson()}, result={grantResponse.ToJson()}");
                }
            }
            return true;
        }

        private static int[] GetParentRewards()
        {
            var parentRewards = new int[21];
            parentRewards[1] = 2;
            parentRewards[6] = 13;
            parentRewards[13] = 10;
            parentRewards[20] = 25;
            return parentRewards;
        }

        public async Task SendMessageAsync(SignIn signIn, int[] rewards, DateTime endTime)
        {
            var msg = "";
            if (signIn.LeftSignCount == 0)
            {
                msg = $"恭喜您！已累计打卡{signIn.SignCount}次，共获得{signIn.TotalMoney}元红包，已完成全部打卡任务";
            }
            else
            {
                //剩余可得奖励天数
                var days = (endTime.Date - DateTime.Today.Date).Days;
                //总可得奖励数
                int total = rewards.Take(signIn.SignCount + days).Sum();
                msg = new StringBuilder()
                    .Append($"恭喜您！已累计打卡{signIn.SignCount}次，共获得{signIn.TotalMoney}元红包，")
                    .Append($"明天打卡成功可让红包增加至{signIn.TotalMoney + signIn.NextRewardMoney}元，")
                    .Append($"距离{total}元红包还差{days}次")
                    .ToString();
            }

            await _capPublisher.PublishAsync(nameof(WeChatMsgIntegrationEvent), new WeChatMsgIntegrationEvent()
            {
                UserId = signIn.MemberId,
                Msg = msg
            });
        }

        private async Task SendParentMessageAsync(Guid userId, Guid parentUserId, int[] parentRewards, int parentInviteCount, int parentTotalMoney, DateTime endTime)
        {
            var user = await _marketingAPIClient.GetWxOpenInfo(userId);
            var userName = user?.UserName ?? "";

            //剩余可得奖励天数
            var days = (endTime.Date - DateTime.Today.Date).Days;
            var totalDays = parentInviteCount + days;
            //总可得奖励数
            int total = 0;// parentRewards.Take(parentInviteCount + days).Sum();
            int leftCount = 0;
            for (int i = 0; i < totalDays; i++)
            {
                int item = parentRewards[i];
                if (item > 0)//最后一次获得奖励的次数
                {
                    //距离最后一次奖励的次数
                    leftCount = i + 1 - parentInviteCount;
                }
                total += item;
            }

            var msg = "";
            //if (parentInviteCount >= parentRewards.Length)
            if (leftCount <= 0)
            {
                msg = $"恭喜您！已累计邀请{userName}打卡{parentInviteCount}次，共获得{parentTotalMoney}元红包，已完成邀请{userName}的全部任务";
            }
            else
            {
                //var total = parentRewards.Sum();
                int leftInviteCount = parentRewards.Length - parentInviteCount;
                int nextLeftInviteCount = 0;
                int nextInviteMoney = 0;
                for (int i = 0; i < parentRewards.Length; i++)
                {
                    if (i + 1 > parentInviteCount && parentRewards[i] > 0)
                    {
                        nextLeftInviteCount = i + 1 - parentInviteCount;
                        nextInviteMoney = parentRewards[i];
                        break;
                    }
                }
                int nextTotal = parentTotalMoney + nextInviteMoney;


                msg = new StringBuilder()
                    .Append($"恭喜您！已累计邀请{userName}打卡{parentInviteCount}次，共获得{parentTotalMoney}元红包，")
                    .Append($"再邀请他打卡{nextLeftInviteCount}次可让红包增加至{nextTotal}元，")
                    .Append($"距离{total}元红包还差{leftCount}次")
                    .ToString();
            }

            await _capPublisher.PublishAsync(nameof(WeChatMsgIntegrationEvent), new WeChatMsgIntegrationEvent()
            {
                UserId = parentUserId,
                Msg = msg
            });
        }

        public async Task<APIResult<WalletResponse>> GrantMoney(Guid userId, int money, string remark)
        {
            var resp = await _financeCenterAPIClient.FreezeAmountIncome(new WalletRequest()
            {
                UserId = userId,
                BlockedAmount = money,
                Remark = remark
            });
            resp.Data ??= new WalletResponse();

            if (!(resp.Succeed && resp.Data.Success))
            {
                _logger.LogError("发放签到奖励失败, " + resp.ToJson());
            }
            return resp;
        }
    }
}
