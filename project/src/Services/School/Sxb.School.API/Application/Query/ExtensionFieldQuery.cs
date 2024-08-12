using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class ExtensionFieldQuery : IExtensionFieldQuery
    {
        public Task<IEnumerable<SchoolYearFieldContentInfo>> ListContentAsync(Guid eid, int year = 0, string field = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SchoolYearFieldContentInfo>> ListOnlineContentAsync(Guid eid, int year = 0, string field = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OnlineYearExtFieldInfo>> ListOnlineYearsAsync(Guid eid, string field = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<YearExtFieldInfo>> ListYearsAsync(Guid eid, string field = null)
        {
            throw new NotImplementedException();
        }
    }
}
