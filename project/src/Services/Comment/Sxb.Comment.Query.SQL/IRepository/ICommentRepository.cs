using Sxb.Comment.Common.DTO;
using Sxb.Comment.Common.Entity;
using Sxb.Comment.Common.EntityExtend;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.Comment.Query.SQL.IRepository
{
    public interface ICommentRepository
    {
        Task<IEnumerable<SchoolCommentInfo>> GetSchoolSelectedComment(Guid eid, int order, int take = 0);
        Task<IEnumerable<SchoolCommentScoreInfo>> GetCommentScoresByCommentIDs(IEnumerable<Guid> commentIDs);
        Task<IEnumerable<CommentTagExtend>> GetCommentTagsByCommentIDs(IEnumerable<Guid> commentIDs);
        Task<IEnumerable<SchoolCommentTotalDTO>> GetCommentTotalBySID(Guid eid);

        Task<IEnumerable<SchoolScoreCommentCountDto>> GetSchoolScoreCommentCountByEids(IEnumerable<Guid> eids);
        Task<IEnumerable<SchoolCommentTotalDTO>> GetCommentTotalByEIDsAsync(IEnumerable<Guid> eids);
    }
}