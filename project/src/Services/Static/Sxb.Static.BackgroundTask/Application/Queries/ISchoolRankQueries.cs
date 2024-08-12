using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Application.Queries
{
    public interface ISchoolRankQueries
    {
        Task<SchoolRank> GetSchoolRankFromNoAsync(long no);
    }
}
