using Sxb.Comment.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Comment.API.Application.Query
{
    public interface ICommentQuery
    {
        Task<IEnumerable<SchoolCommentDTO>> GetCommentsByEID(Guid eid, Guid? userID, int take = 1);
        int GetCurrentSchoolstart(decimal scoreTemp);
        Task<IEnumerable<SchoolCommentTotalDTO>> GetCommentTotalBySID(Guid eid);

        /// <summary>
        /// 获取学校评分(家长评价)共n人点评
        /// </summary>
        Task<IEnumerable<SchoolScoreCommentCountDto>> GetSchoolScoreCommentCountByEids(IEnumerable<Guid> eids);
        Task<IEnumerable<SchoolCommentTotalDTO>> GetCommentTotalByEIDsAsync(IEnumerable<Guid> eids);
    }
}
