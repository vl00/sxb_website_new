using Kogel.Dapper.Extension.MsSql;
using Sxb.Comment.Common.DB;
using Sxb.Comment.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Comment.Query.SQL.IRepository
{
    public class SchoolGiveLikeRepository : ISchoolGiveLikeRepository
    {
        readonly SchoolProductDB _schoolProductDB;

        public SchoolGiveLikeRepository(SchoolProductDB schoolProductDB)
        {
            _schoolProductDB = schoolProductDB;
        }

        public async Task<IEnumerable<GiveLikeInfo>> CheckCurrentUserIsLikeBulk(Guid userID, IEnumerable<Guid> sourceIDs)
        {
            if (userID == default || sourceIDs == null || !sourceIDs.Any()) return null;
            return await _schoolProductDB.SlaveConnection.QuerySet<GiveLikeInfo>().Where(p => p.UserID == userID).Where($"SourceID In ('{string.Join("','", sourceIDs)}')").ToIEnumerableAsync();
        }
    }
}
