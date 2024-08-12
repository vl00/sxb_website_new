using Sxb.Infrastructure.Core;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.IRepository
{
    public interface ISchoolRepository 
    {

        Task<IEnumerable<School>> GetValidAfterAsync(DateTime preUpdateTime, int offset, int limit);
        Task<(Guid id1, Guid id2)> GetIdByNo(long no1, long no2);
    }
}
