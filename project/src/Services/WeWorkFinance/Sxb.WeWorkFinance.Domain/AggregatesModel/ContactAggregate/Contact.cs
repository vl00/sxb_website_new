using Sxb.Domain;
using Sxb.WeWorkFinance.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate
{
    public class Contact : Entity<string>, IAggregateRoot
    {
        public string Name { get; private set; }

        public string Avatar { get; private set; }

        public string Position { get; private set; }
        public string CorpName { get; private set; }
        public string CorpFullName { get; private set; }
        public int Type { get; private set; }
        public int Gender { get; private set; }

        public string UnionId { get; private set; }

        public string State { get; private set; }
        public string EmplyeeUserid { get; private set; }

        public string ParentUserid { get; private set; }

        public bool IsJoinGroup { get; private set; }

        /// <summary>
        /// 是否为上一次活动的被发展人
        /// </summary>
        public bool IsLastActivity { get; private set; }

        public DateTime CreateTime { get; private set; }

        public void Create(string id, string name, string avatar, string position, string corpName, string corpFullName, int type, int gender, string unionId, string state, string emplyeeUserid, string parentUserid,Guid adviserUserId)
        {
            Id = id;
            Name = name;
            Avatar = avatar;
            Position = position;
            CorpName = corpName;
            CorpFullName = corpFullName;
            Type = type;
            Gender = gender;
            UnionId = unionId;
            State = state;
            EmplyeeUserid = emplyeeUserid;
            ParentUserid = parentUserid;
            CreateTime = DateTime.Now;
            IsLastActivity = false;

            //领域事件：顾问绑定被发展人
            this.AddDomainEvent(new BindAdviserDomainEvent(unionId,name, gender, avatar, adviserUserId));
        }

        public void JoinGroup()
        {
            IsJoinGroup = true;
        }
    }
}
