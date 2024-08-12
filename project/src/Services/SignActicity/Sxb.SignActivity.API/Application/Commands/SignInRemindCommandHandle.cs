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
    public class SignInRemindCommandHandle : IRequestHandler<SignInRemindCommand, ResponseResult>
    {
        private readonly ILogger<SignInRemindCommand> _logger;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ICapPublisher _capPublisher;

        private readonly ISignInRepository _signInRepository;
        private readonly ISignInHistoryRepository _signInHistoryRepository;
        private readonly ISignInParentHistoryRepository _signInParentHistoryRepository;
        private readonly SignActivity.Query.SQL.IRepository.ISignConfigRepository _signConfigRepository;
        private readonly SignActivity.Query.SQL.IRepository.IOrderRepository _orderRepository;

        private readonly IFinanceCenterAPIClient _financeCenterAPIClient;
        private readonly IMarketingAPIClient _marketingAPIClient;

        public SignInRemindCommandHandle(ILogger<SignInRemindCommand> logger, IEasyRedisClient easyRedisClient, ISignInRepository signInRepository, ISignInHistoryRepository signInHistoryRepository, SignActivity.Query.SQL.IRepository.ISignConfigRepository signConfigRepository, SignActivity.Query.SQL.IRepository.IOrderRepository orderRepository, IFinanceCenterAPIClient financeCenterAPIClient, IMarketingAPIClient marketingAPIClient, ICapPublisher capPublisher, ISignInParentHistoryRepository signInParentHistoryRepository)
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

        public async Task<ResponseResult> Handle(SignInRemindCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogError("签到提醒数据为空");
            }
            _logger.LogInformation($"开始签到提醒data={request.ToJson()},now={DateTime.Now}");

            var key = RedisKeys.SignInRemindLockKey;
            var locked = await _easyRedisClient.LockTakeAsync(key, "1", TimeSpan.FromSeconds(10));
            if (locked)
            {
                try
                {
                    return await Main(request, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return ResponseResult.Failed(ex.Message);
                }
                finally
                {
                    //release lock
                    await _easyRedisClient.LockReleaseAsync(key, "1");
                }
            }
            else
            {
                return LogError("操作频繁, 请稍后再试");
            }
        }

        public async Task<ResponseResult> Main(SignInRemindCommand request, CancellationToken cancellationToken)
        {
            //最大参与人数
            var buNo = BuNoType.SHUANG11_ACTIVITY.ToString();
            IEnumerable<Guid> userIds = request.UserIds;
            if (userIds == null || !userIds.Any())
            {
                //获取所有顾问
                userIds = await _marketingAPIClient.GetConsultantUserIds();
            }

            if (!userIds.Any())
            {
                return LogError("发送签到提醒, 无用户数据");
            }

            //check activity
            var signConfig = await _signConfigRepository.GetAsync(buNo);
            if (signConfig == null)
            {
                return LogError("发送签到提醒, 无签到配置");
            }
            if (signConfig.UserLimit == 0)
            {
                return LogError("发送签到提醒, 签到名额配置为0");
            }

            //实际打卡名额
            int signInReal = signConfig.UserLimit;
            //实际实时打卡人数
            int nowSignInReal = _signInRepository.GetSignUserCount(buNo);
            //实际实时剩余打卡名额
            int signInSurplus = signInReal - nowSignInReal;
            if (signInSurplus <= 0)
            {
                return LogError("发送签到提醒, 签到名额已用尽");
            }

            //宣传打卡名额
            int signInVirtual = 10000;
            //宣传实时剩余打卡人数
            int signInSurplusVirtual = signInVirtual * signInSurplus / signInReal;

            //实际打卡名额
            int evalustionReal = signConfig.UserLimit/2;
            //宣传种草名额
            int evaluationVirtual = 5000;
            //宣传实时种草奖励人数
            int evaluationSurplusVirtual = evaluationVirtual - evaluationVirtual * nowSignInReal / evalustionReal;

            evaluationSurplusVirtual = evaluationSurplusVirtual < 0 ? 0 : evaluationSurplusVirtual;

            //亲爱的顾问用户，上学帮温馨提醒你，1w打卡名额已剩6667，10元种草奖励名额已剩1667，名额有限快去邀请。
            var msg = $"亲爱的顾问用户，上学帮温馨提醒你，{signInVirtual}打卡名额仅剩{signInSurplusVirtual}，10元种草奖励名额仅剩{evaluationSurplusVirtual}，名额有限快去邀请。";
            if (evaluationSurplusVirtual == 0)
            {
                msg = $"亲爱的顾问用户，上学帮温馨提醒你，{signInVirtual}打卡名额仅剩{signInSurplusVirtual}，10元种草奖励名额已抢光，名额有限快去邀请。";
            }

            if (request.IsSendMsg)
            {
                foreach (var userId in userIds)
                {
                    await _capPublisher.PublishAsync(nameof(WeChatMsgIntegrationEvent), new WeChatMsgIntegrationEvent()
                    {
                        UserId = userId,
                        TemplateFirst = "温馨提示",
                        FailTrySMS = false,
                        IsBlink = false,
                        Msg = msg
                    });
                }
            }
            return ResponseResult.Success(new
            {
                buNo,
                msg,
                userIds
            });
        }

        public ResponseResult LogError(string errmsg)
        {
            _logger.LogError(errmsg);
            return ResponseResult.Failed(errmsg);
        }
    }
}
