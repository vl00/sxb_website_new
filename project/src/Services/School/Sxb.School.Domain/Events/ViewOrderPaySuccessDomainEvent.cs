using Sxb.Domain;
using Sxb.School.Domain.AggregateModels.ViewOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Domain.Events
{
    public class ViewOrderPaySuccessDomainEvent:IDomainEvent
    {
        public Guid OrderId { get; set; }
        public string OrderNo { get; set; }

        public Guid UserId { get; set; }

        public ViewSchoolGoodsInfo  ViewSchoolGoodsInfo { get; set; }

    }
}
