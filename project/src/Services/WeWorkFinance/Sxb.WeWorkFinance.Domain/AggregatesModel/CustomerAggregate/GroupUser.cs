using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate
{
    public class GroupUser : Entity<string>, IAggregateRoot
    {
        public string OpenId { get; private set; }
        public string UnoinId { get; private set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public string CorpName { get; set; }
        public string CorpFullName { get; set; }
        public int Type { get; set; }

        public int Gender { get; set; }

        public void Create(string id, string openId)
        {
            Id = id;
            OpenId = openId;
        }
    }
}
