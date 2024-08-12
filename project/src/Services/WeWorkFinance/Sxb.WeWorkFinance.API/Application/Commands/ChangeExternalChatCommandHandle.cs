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
using Sxb.WeWorkFinance.API.Config;
using Microsoft.Extensions.Options;
using Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using Sxb.WeWorkFinance.API.Application.Queries;

namespace Sxb.WeWorkFinance.API.Application.Commands
{
    public class ChangeExternalChatCommandHandle : IRequestHandler<ChangeExternalChatCommand, bool>
    {
        private readonly WorkWeixinConfig _workConfig;
        private readonly InviteActivityConfig _inviteActivityConfig;
        private readonly IGroupChatRepository _chatRepository;
        private readonly IGroupMemberRepository _memberRepository;
        private readonly IContactRepository _contactRepository;
        private readonly ICustomerQrCodeRepository _customerQrCodeRepository;
        private readonly IWeixinQueries _weixinQueries;
        private readonly IMediator _mediator;
        private readonly IEasyRedisClient _easyRedisClient;

        private readonly ILogger<ChangeExternalChatCommandHandle> _logger;

        public ChangeExternalChatCommandHandle(IOptions<WorkWeixinConfig> workConfig, IOptions<InviteActivityConfig> inviteActivityConfig,
            ILogger<ChangeExternalChatCommandHandle> logger, IGroupChatRepository chatRepository, IGroupMemberRepository memberRepository, IWeixinQueries weixinQueries,
            IContactRepository contactRepository, ICustomerQrCodeRepository customerQrCodeRepository,IEasyRedisClient easyRedisClient,IMediator mediator)
        {
            _workConfig = workConfig.Value;
            _inviteActivityConfig = inviteActivityConfig.Value;
            _chatRepository = chatRepository;
            _memberRepository = memberRepository;
            _contactRepository = contactRepository;
            _customerQrCodeRepository = customerQrCodeRepository;
            _weixinQueries = weixinQueries;
            _easyRedisClient = easyRedisClient;
            _mediator = mediator;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<bool> Handle(ChangeExternalChatCommand request, CancellationToken cancellationToken)
        {
            string sCorpID = _workConfig.CorpId;
            string sCorpSecret = _workConfig.CorpSecret;
            var options = new WechatWorkClientOptions()
            {
                CorpId = sCorpID
            };

            string groupChatId = request.GroupChatId;
            string accessToken = await _easyRedisClient.GetOrAddAsync($"wxwork:AccessToken:{sCorpID}", () => GetAccessToken.GetToken(sCorpID, sCorpSecret),new TimeSpan(0,20,0));


            //获取客户群的详情
            var client = new WechatWorkClient(options);
            var req= new SKIT.FlurlHttpClient.Wechat.Work.Models.CgibinExternalContactGroupChatGetRequest() { GroupChatId = groupChatId ,AccessToken = accessToken };
            var resp = await client.ExecuteCgibinExternalContactGroupChatGetAsync(req);

            if (resp.ErrorCode == 40014 || resp.ErrorCode == 42001)
            {
                //accessToken失效，重新获取
                accessToken = GetAccessToken.GetToken(sCorpID, sCorpSecret); 
                await _easyRedisClient.AddAsync($"wxwork:AccessToken:{sCorpID}", accessToken);
                req.AccessToken = accessToken;
                resp = await client.ExecuteCgibinExternalContactGroupChatGetAsync(req);
            }
            if (resp.ErrorCode == 49008)
            {
                //群已解散
                return true;
            }

            var chat = _chatRepository.FirstOrDefault(q=>q.Id == groupChatId);
            List<GroupUser> newUsers = new List<GroupUser>();
            if (chat == null)
            {
                chat = new GroupChat();
                chat.Create(resp.GroupChat.GroupChatId, 0, resp.GroupChat.Name, resp.GroupChat.Notice, resp.GroupChat.CreateTimestamp.I2DSecond(), resp.GroupChat.OwnerUserId);
                _chatRepository.Add(chat);
            }
            else 
            {
                chat.Update(resp.GroupChat.Name, resp.GroupChat.Notice, resp.GroupChat.OwnerUserId);
                _chatRepository.Update(chat);
            }

            //现有的群成员
            var members =  _memberRepository.GetAllIQueryable(q => q.ChatId == groupChatId).ToList();
            var memberIds = members.Select(q => q.UserId).ToList();

            //新增的群成员
            var newMember = resp.GroupChat.MemberList.Where(q => !memberIds.Contains(q.UserId));
            List<GroupMember> groupMembers = new List<GroupMember>();


            foreach (var item in newMember)
            {
                var m = new GroupMember();
                m.JoinGroup(groupChatId, item.UnionId, item.Type, item.JoinTimestamp.I2DSecond(),item.JoinScene, item.Invitor?.UserId, item.UserId);
                groupMembers.Add(m);

                //Console.WriteLine("///"+item.UnionId+","+ item.JoinScene + "," + item.Invitor?.UserId+"///");
                if (item.Type == 2 & item.JoinScene == 3 &&  await _weixinQueries.ExistCustomer(item.Invitor?.UserId))
                {
                    //判断新增的群成员是否是被发展人第一次加群
                    Contact contact = _contactRepository.FirstOrDefault(q => q.UnionId == item.UnionId && q.IsJoinGroup == false);
                    if (contact != null)
                    {
                        contact.JoinGroup();
                        await _contactRepository.UpdateAsync(contact, cancellationToken);

                        bool isSuccess = await _contactRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

                        if (isSuccess && contact.Gender == 2)//2021.09.17更新为邀请女性才能新增资格
                        {
                            if (string.IsNullOrWhiteSpace(item.UnionId))
                            {
                                _logger.LogWarning("存在未返回unoinid，客服用户:{0},发展人:{1},被发展人id:{2}", item.Invitor?.UserId, contact.ParentUserid, item.UserId);
                                continue;
                            }

                            //记录下添加客服的用户以及上级用户的关系，
                            //如果之前已存在添加客服的用户的旧记录则不新增记录，并且不新增购买资格

                            _logger.LogInformation("进群[{3}]记录，客服用户:{0},发展人:{1},被发展人:{2}", item.Invitor?.UserId, contact.ParentUserid, item.UnionId, groupChatId);

                            string inviterKey = "org:InviteActivity:InviterBonusPoint";
                            string inviteeKey = "org:InviteActivity:InviteeBuyQualify";

                            //新增邀请者的购买积分+1
                            await _easyRedisClient.SortedSetIncrementAsync(inviterKey, contact.ParentUserid, 1);
                            //第一次添加客服的用户获得购买资格
                            await _easyRedisClient.SortedSetAddAsync(inviteeKey, item.UnionId, 1);

                            var customerQr = _customerQrCodeRepository.FirstOrDefault(q => q.InviterId == contact.ParentUserid);
                            if (customerQr != null)
                            {
                                //公众号向发展人发送消息
                                await _mediator.Send(new SuccessInviteMsgCommand(customerQr.InviterOpenId, customerQr.InviterId), cancellationToken);
                            }
                        }
                    }
                }
            }
            _memberRepository.AddRange(groupMembers);


            var nowMemberIds = resp.GroupChat.MemberList.Select(q => q.UserId);
            //重新加入的群成员
            var rejoinMember = members.Where(q => q.IsExit == true && nowMemberIds.Contains(q.UserId));
            foreach (var item in rejoinMember)
            {
                item.RejoinGroup();
                await _memberRepository.UpdateAsync(item);
            }

            //退出的群成员
            var exitMember = members.Where(q => !nowMemberIds.Contains(q.UserId));
            foreach (var item in exitMember)
            {
                item.ExitGroup();
                await _memberRepository.UpdateAsync(item);
            }

            await _chatRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            await _memberRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            
            return true;
        }
    }
}
