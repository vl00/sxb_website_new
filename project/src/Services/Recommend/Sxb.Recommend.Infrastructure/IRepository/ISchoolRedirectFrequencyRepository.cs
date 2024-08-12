using MongoDB.Bson;
using Sxb.Infrastructure.Core;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.IRepository
{
    public interface ISchoolRedirectFrequencyRepository : IRepository<SchoolRedirectFrequency, ObjectId>
    {


        Task<SchoolRedirectFrequency> QueryFrequencyAsync(Guid sidp, Guid sids);
        Task<IEnumerable<SchoolRedirectFrequency>> QueryFrequenciesAsync(int offset, int limit);
        Task<IEnumerable<SchoolRedirectFrequency>> QueryFrequenciesAsync(Guid sidp);

    }
}
