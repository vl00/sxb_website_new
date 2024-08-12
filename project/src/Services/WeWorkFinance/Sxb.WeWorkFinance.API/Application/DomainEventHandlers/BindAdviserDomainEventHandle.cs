using Sxb.Domain;
using Sxb.WeWorkFinance.API.Application.HttpClients;
using Sxb.WeWorkFinance.API.Application.Queries;
using Sxb.WeWorkFinance.API.Application.Queries.ViewModels;
using Sxb.WeWorkFinance.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Application.DomainEventHandlers
{
    public class BindAdviserDomainEventHandle : IDomainEventHandler<BindAdviserDomainEvent>
    {
        private readonly IUserQueries _userQueries;
        private readonly MarketingServerClient _marketingServerClient;
        public BindAdviserDomainEventHandle(IUserQueries userQueries, MarketingServerClient marketingServerClient)
        {
            _userQueries = userQueries;
            _marketingServerClient = marketingServerClient;
        }

        public async Task Handle(BindAdviserDomainEvent @event, CancellationToken cancellationToken)
        {
            //查询被发展人的用户信息，不存在则新增一个用户
            var inviteeUser = await _userQueries.GetUserInfo(@event.UnionId);

            bool flag = true;

            if (inviteeUser == null)
            {
                Guid inviteeUserId = Guid.NewGuid();

                inviteeUser = new UserInfoViewModel()
                {
                    UserId = inviteeUserId,
                    NickName = @event.NickName,
                    UnionId = @event.UnionId
                };
                int? sex = null;
                if (@event.Gender == 1)
                {
                    sex = 0;
                }
                else if (@event.Gender == 2)
                {
                    sex = 1;
                }
                flag = await _userQueries.AddUserInfo(@event.UnionId, inviteeUserId, @event.NickName, sex, @event.AvatarUrl);
            }

            if (flag)
            {
                //预锁粉
                await _marketingServerClient.PreLockFans(inviteeUser.UserId, @event.AdviserUserId);
            }
        }
    }
}
