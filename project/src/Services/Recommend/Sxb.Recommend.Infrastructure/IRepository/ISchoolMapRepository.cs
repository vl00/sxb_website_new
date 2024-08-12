using MongoDB.Bson;
using Sxb.Infrastructure.Core;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.IRepository
{
    public interface ISchoolMapRepository: IRepository<SchoolMap,ObjectId>
    {
        Task<bool> HasSchoolMaps(School school);
        Task<IEnumerable<SchoolMap>> GetSchoolMaps(School school,int offset,int limit);
        Task<IEnumerable<SchoolMap>> GetSchoolMaps(Guid eid, int offset, int limit);
        Task InsertManyAsync(IEnumerable<SchoolMap> documents);

        Task InsertAsync(SchoolMap document);
        Task ClearAll();
        Task<IEnumerable<SchoolMap>> GetSchoolMaps(School schoolS);
    }
}
