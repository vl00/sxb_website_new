using Sxb.Infrastructure.Core;
using Sxb.SignActivity.Domain.AggregatesModel.SignAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.SignActivity.Infrastructure.Repositories
{
    public class SignConfigRepository : Repository<SignConfig, Guid, OrganizationContext>, ISignConfigRepository
    {
        public SignConfigRepository(OrganizationContext context) : base(context)
        {
        }


        public async Task<SignConfig> GetAsync(string buNo)
        {
            var data = FirstOrDefault(s => s.BuNo == buNo && s.Isvalid == true);
            return await Task.FromResult(data);
        }
    }
}
