using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate
{
    /// <summary>
    /// 顾问群
    /// </summary>
    public class AdviserGroup : Entity<string>, IAggregateRoot
    {
        public string UnionId { get; private set; }
        public string GroupQrCodeUrl { get; private set; }
        public string CustomerId { get; set; }
    }
}
