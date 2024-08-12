using MongoDB.Bson;
using Sxb.Infrastructure.Core;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.IRepository
{
    public interface ISchoolRedirectInsideRepository : IRepository<SchoolRedirectInside,ObjectId>
    {
        Task<List<SchoolRedirectInside>> ListAsync(Expression<Func<SchoolRedirectInside, bool>> whereLambda = null);
        Task<SchoolRedirectFrequency> QueryFrequencyAsync(Guid sidp, Guid sids);


    }
}
