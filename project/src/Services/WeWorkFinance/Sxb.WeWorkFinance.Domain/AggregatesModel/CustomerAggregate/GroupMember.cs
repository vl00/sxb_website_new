using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate
{
    public class GroupMember : Entity<Guid>, IAggregateRoot
    {
        public string ChatId { get; private set; }
        public string UserId { get; private set; }

        public string UnoinId { get; private set; }

        public int MemberType { get; private set; }

        public DateTime JoinTime { get; private set; }

        public int JoinSence { get; private set; }

        public string InvitorUserId { get; private set; }

        public bool IsExit { get; set; }

        public bool IsNewInvitJoin { get; set; }

        public virtual GroupChat Chat{ get; private set; }

        public void JoinGroup(string chatId, string unoinId,int memberType, DateTime joinTime,int joinSence,string invitorUserId, string userId)
        {
            Id = Guid.NewGuid();
            ChatId = chatId;
            UnoinId = unoinId;
            MemberType = memberType;
            JoinTime = joinTime;
            JoinSence = joinSence;
            InvitorUserId = invitorUserId;
            UserId = userId;

            IsNewInvitJoin = !string.IsNullOrWhiteSpace(invitorUserId) && invitorUserId!= userId;
        }

        public void RejoinGroup()
        {
            IsExit = false;
        }

        public void ExitGroup()
        {
            IsExit = true;
        }
    }
}
