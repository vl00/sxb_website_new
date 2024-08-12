using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.Comment.Common.OtherAPIClient.School
{
    public interface ISchoolAPIClient
    {
        Task<IEnumerable<Guid>> GetValidEIDs(Guid sid);
    }
}
