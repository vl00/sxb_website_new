using Sxb.Comment.Common.DTO;
using Sxb.Comment.Common.Entity;
using Sxb.Comment.Common.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.Comment.Query.SQL.IRepository
{
    public interface ISchoolGiveLikeRepository
    {
        Task<IEnumerable<GiveLikeInfo>> CheckCurrentUserIsLikeBulk(Guid userID, IEnumerable<Guid> sourceIDs);
    }
}
