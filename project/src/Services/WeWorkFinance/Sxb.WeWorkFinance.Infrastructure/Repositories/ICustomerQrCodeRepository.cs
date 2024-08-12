using Sxb.Infrastructure.Core;
using Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Infrastructure.Repositories
{
    public interface ICustomerQrCodeRepository : IRepository<CustomerQrCode, string>
    {
    }
}
