using Microsoft.EntityFrameworkCore;
using Sxb.Infrastructure.Core;
using Sxb.SignActivity.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order, Guid, OrganizationContext>, IOrderRepository
    {
        public OrderRepository(OrganizationContext context) : base(context)
        {
        }
    }
}
