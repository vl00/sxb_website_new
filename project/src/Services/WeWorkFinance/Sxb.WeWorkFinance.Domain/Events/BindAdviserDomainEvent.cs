using Sxb.Domain;
using Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Domain.Events
{
    public class BindAdviserDomainEvent : IDomainEvent
    {
        public string UnionId { get; set; }
        public string NickName { get; set; }
        public int Gender { get; set; }
        public string AvatarUrl { get; set; }
        public Guid AdviserUserId { get; set; }
        public BindAdviserDomainEvent(string unionId, string nickName, int gender, string avatarUrl, Guid adviserUserId)
        {
            UnionId = unionId;
            NickName = nickName;
            Gender = gender;
            AvatarUrl = avatarUrl;
            AdviserUserId = adviserUserId;
        }
    }
}
