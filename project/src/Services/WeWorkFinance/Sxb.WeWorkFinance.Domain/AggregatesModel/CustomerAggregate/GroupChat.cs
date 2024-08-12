using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate
{
    /// <summary>
    /// 微信企业群
    /// </summary>
    public class GroupChat : Entity<string>, IAggregateRoot
    {
        public int Status { get; private set; }
        public string GroupName { get; private set; }
        public string GroupOwnerUserId { get; private set; }
        public DateTime CreateTime { get; private set; }
        public string Notice { get; private set; }

        public GroupChat()
        {
        }

        public GroupChat(string id , int status)
        {
            Id = id;
            Status = status;
        }

        public void Create(string id,int status, string groupName ,string notice, DateTime createTime, string groupOwnerUserId)
        {
            Id = id;
            Status = status;
            GroupName = groupName;
            Notice = notice;
            CreateTime = createTime;
            GroupOwnerUserId = groupOwnerUserId;
        }

        public void Update(string groupName, string notice , string groupOwnerUserId)
        {
            GroupName = groupName;
            Notice = notice;
            GroupOwnerUserId = groupOwnerUserId;
        }

    }
}
