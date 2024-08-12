using Sxb.Infrastructure.Core;
using Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate;

namespace Sxb.WeWorkFinance.Infrastructure.Repositories
{
    public class CustomerQrCodeRepository : Repository<CustomerQrCode, string, UserContext>, ICustomerQrCodeRepository
    {
        public CustomerQrCodeRepository(UserContext context) : base(context)
        {
        }
    }
}
