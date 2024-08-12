using Microsoft.EntityFrameworkCore.Storage;
using Sxb.Infrastructure.Core;
using Sxb.WeWorkFinance.Domain.AggregatesModel.ContactAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.WeWorkFinance.Infrastructure.Repositories
{
    public class ContactRepository : Repository<Contact, string, UserContext>, IContactRepository
    {
        public ContactRepository(UserContext context) : base(context)
        {
        }
    }
}
