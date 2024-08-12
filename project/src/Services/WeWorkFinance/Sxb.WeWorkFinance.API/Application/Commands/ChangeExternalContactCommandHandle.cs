using DotNetCore.CAP;
using MediatR;
using SKIT.FlurlHttpClient.Wechat.Work;
using Sxb.WeWorkFinance.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sxb.Framework.Foundation;
using Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate;
using Sxb.Framework.Cache.Redis;
using Sxb.WeWorkFinance.API.Infrastructure;
using SKIT.FlurlHttpClient.Wechat.Work.Models;
using Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate;
using Sxb.WeWorkFinance.API.Config;
using Microsoft.Extensions.Options;
using Sxb.WeWorkFinance.API.Application.Models;
using Sxb.WeWorkFinance.API.Application.Queries;
using EasyWeChat;
using EasyWeChat.Interface;
using EasyWeChat.CustomMessage;
using System.Text;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using Sxb.WeWorkFinance.API.Application.Queries.ViewModels;

namespace Sxb.WeWorkFinance.API.Application.Commands
{
    public class ChangeExternalContactCommandHandle : IRequestHandler<ChangeExternalContactCommand, bool>
    {
        private readonly WorkWeixinConfig _workConfig;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IWeixinQueries _weixinQueries;
        private readonly IContactRepository _contactRepository;

        private readonly InviteActivityConfig _inviteActivityConfig;

        private readonly ILogger<ChangeExternalContactCommandHandle> _logger;

        public ChangeExternalContactCommandHandle(IWeixinQueries weixinQueries, 
            IContactRepository contactRepository, ILogger<ChangeExternalContactCommandHandle> logger,
            IEasyRedisClient easyRedisClient, IOptions<InviteActivityConfig> inviteActivityConfig, IOptions<WorkWeixinConfig> workConfig)
        {
            _contactRepository = contactRepository;
            _weixinQueries = weixinQueries;
            _easyRedisClient = easyRedisClient;
            _workConfig = workConfig.Value;
            _inviteActivityConfig = inviteActivityConfig.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<bool> Handle(ChangeExternalContactCommand request, CancellationToken cancellationToken)
        {
            string sCorpID = _workConfig.CorpId;
            string sCorpSecret = _workConfig.CorpSecret;
            var options = new WechatWorkClientOptions()
            {
                CorpId = sCorpID
            };
            var client = new WechatWorkClient(options);
            string accessToken = await _easyRedisClient.GetOrAddAsync($"wxwork:AccessToken:{sCorpID}", () => GetAccessToken.GetToken(sCorpID, sCorpSecret), new TimeSpan(0, 20, 0));

            if (request.Event.ActionType == "msg_audit_approved")//客户同意进行聊天内容存档事件回调
            {
                
            }

            return true;
        }
    }
}
