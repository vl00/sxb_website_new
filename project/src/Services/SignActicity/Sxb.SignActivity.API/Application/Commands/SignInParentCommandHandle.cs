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
    public class SignInParentCommandHandle : IRequestHandler<SignInParentCommand, ResponseResult>
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

        public SignInParentCommandHandle(ILogger<SignInCommandHandle> logger, IEasyRedisClient easyRedisClient, ISignInRepository signInRepository, ISignInHistoryRepository signInHistoryRepository, SignActivity.Query.SQL.IRepository.ISignConfigRepository signConfigRepository, SignActivity.Query.SQL.IRepository.IOrderRepository orderRepository, IFinanceCenterAPIClient financeCenterAPIClient, IMarketingAPIClient marketingAPIClient, ICapPublisher capPublisher, ISignInParentHistoryRepository signInParentHistoryRepository)
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

        public async Task<ResponseResult> Handle(SignInParentCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogError("父级签到数据为空");
            }
            _logger.LogInformation($"开始父级签到data={request.ToJson()},now={DateTime.Now}");
        
            var result = await Main(request, cancellationToken);
            return ResponseResult.Success(result);
        }
        public async Task<bool> Main(SignInParentCommand request, CancellationToken cancellationToken)
        {
            //最大参与人数
            var buNo = BuNoType.SHUANG11_ACTIVITY.ToString();
            Guid userId = request.UserId;

            var signConfig = await _signConfigRepository.GetAsync(buNo);
            //check activity
            if (signConfig == null) { throw new Exception("活动不存在"); }

            //打卡统计
            var signIn = await _signInRepository.GetUserSignAsync(buNo, userId);
            var signInHistories = (await _signInHistoryRepository.GetListAsync(buNo, userId)).ToList();

            //发放上级奖励
            await DispatchParentReward(signIn, signConfig.StartTime, signConfig.EndTime);

            return true;
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
