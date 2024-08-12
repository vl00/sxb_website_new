using DotNetCore.CAP;
using MediatR;
using Sxb.Framework.Cache.Redis;
using SKIT.FlurlHttpClient.Wechat.Work;
using SKIT.FlurlHttpClient.Wechat.Work.Models;
using Sxb.WeWorkFinance.API.Infrastructure;
using Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate;
using Sxb.WeWorkFinance.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sxb.Framework.Foundation;

namespace Sxb.WeWorkFinance.API.Application.Commands
{
    public class GroupchatInitCommandHandle : IRequestHandler<GroupchatInitCommand, bool>
    {
        IGroupChatRepository _chatRepository;
        IGroupMemberRepository _memberRepository;

        private readonly IEasyRedisClient _easyRedisClient;
        ICapPublisher _capPublisher;
        public GroupchatInitCommandHandle(IGroupChatRepository chatRepository, IGroupMemberRepository memberRepository, IEasyRedisClient easyRedisClient, ICapPublisher capPublisher)
        {
            _chatRepository = chatRepository;
            _memberRepository = memberRepository;
            _capPublisher = capPublisher;

            _easyRedisClient = easyRedisClient;
        }
        public async Task<bool> Handle(GroupchatInitCommand request, CancellationToken cancellationToken)
        {
            string sCorpID = "ww3f6a2088ec08814d";
            string sCorpSecret = "82JGzITaTsiX4Mj58I3Z7YXwETKOOOhDNz9bP8lkiQ4";

            string accessToken = await _easyRedisClient.GetOrAddAsync($"wxwork:AccessToken:{sCorpID}",() => GetAccessToken.GetToken(sCorpID, sCorpSecret));

            var options = new WechatWorkClientOptions()
            {
                CorpId = sCorpID
            };

            var client = new WechatWorkClient(options);



            var req = new CgibinExternalContactGroupChatListRequest() { 
                AccessToken = accessToken,
                Limit = 1000
            };

            bool next = true;

            List<GroupChat> chats = new List<GroupChat>();

            while (next)
            {
                var resp = await client.ExecuteCgibinExternalContactGroupChatListAsync(req, cancellationToken: cancellationToken);

                if (resp.ErrorCode == 40014 || resp.ErrorCode == 42001)
                {
                    accessToken = GetAccessToken.GetToken(sCorpID, sCorpSecret);
                    await _easyRedisClient.AddAsync($"wxwork:AccessToken:{sCorpID}", accessToken);
                    continue;
                }

                foreach (var item in resp.GroupChatList)
                {
                    chats.Add(new GroupChat(item.GroupChatId,item.Status));
                }


                if (string.IsNullOrWhiteSpace(resp.NextCursor))
                {
                    next = false;
                }
                else
                {
                    req.NextCursor = resp.NextCursor;
                }
            }
            

            List<GroupMember> groupMembers = new List<GroupMember>();
            foreach (var chat in chats)
            {
                string groupChatId = chat.Id;

                //获取客户群的详情
                var chatDetailReq = new SKIT.FlurlHttpClient.Wechat.Work.Models.CgibinExternalContactGroupChatGetRequest() { GroupChatId = groupChatId, AccessToken = accessToken };
                var resp = await client.ExecuteCgibinExternalContactGroupChatGetAsync(chatDetailReq, cancellationToken: cancellationToken);

                chat.Update(resp.GroupChat.Name, resp.GroupChat.Notice, resp.GroupChat.OwnerUserId);
                foreach (var item in resp.GroupChat.MemberList)
                {
                    var m = new GroupMember();
                    m.JoinGroup(groupChatId,item.UnionId, item.Type, item.JoinTimestamp.I2DSecond(), item.JoinScene, item.Invitor?.UserId, item.UserId);
                    groupMembers.Add(m);
                }
                _memberRepository.AddRange(groupMembers);
            }
            _chatRepository.AddRange(chats);

            await _chatRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            await _memberRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return true;
        }
    }
}
