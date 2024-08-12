using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Domain.AggregatesModel.CustomerAggregate
{
    public class GiveLuckyMoney : Entity<Guid>, IAggregateRoot
    {
        public string UserId { get; private set; }
        public decimal Money { get; private set; }

        public DateTime CreateTime { get; set; }

        public string ReturnCode { get; private set; }
        public string ReturnMsg { get; private set; }

        public void Send(string userId, decimal money)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Money = money;
        }

        public void Result(string returnCode, string returnMsg)
        {
            ReturnCode = returnCode;
            ReturnMsg = returnMsg;
        }
    }
}
