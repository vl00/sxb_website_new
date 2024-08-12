using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Domain.AggregatesModel.LogAggregate
{
    public class LogCloseOrder : Entity<int>, IAggregateRoot
    {
        public Guid OrderId { get; set; }
        public Guid OrderDetailId { get; set; }
        public int Count { get; set; }
        public Guid UserId { get; set; }
        public string UnionId { get; set; }
        public int ConsumedScores { get; set; }
        public DateTime CreateTime { get; set; }
        public bool Succeed { get; set; }


        public void Add(Guid orderId, Guid orderDetailId, int count, Guid userId, string unionId, int consumedScores, DateTime createTime, bool succeed)
        {
            OrderId = orderId;
            OrderDetailId = orderDetailId;
            Count = count;
            UserId = userId;
            UnionId = unionId;
            ConsumedScores = consumedScores;
            CreateTime = createTime;
            Succeed = succeed;
        }
    }
}
