using Sxb.Comment.Common.OtherAPIClient.School;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.Comment.API.Application.Query
{
    public class SchoolQuery : ISchoolQuery
    {
        readonly ISchoolAPIClient _schoolAPIClient;
        public SchoolQuery(ISchoolAPIClient schoolAPIClient)
        {
            _schoolAPIClient = schoolAPIClient;
        }

        public async Task<IEnumerable<Guid>> ListValidEIDsAsync(Guid sid)
        {
            if (sid == default) return default;
            return await _schoolAPIClient.GetValidEIDs(sid);
        }
    }
}
