using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.Comment.API.Application.Query
{
    public interface ISchoolQuery
    {
        Task<IEnumerable<Guid>> ListValidEIDsAsync(Guid sid);
    }
}
