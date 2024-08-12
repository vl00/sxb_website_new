using Sxb.School.Common.OtherAPIClient.Comment.Model.Entity;
using Sxb.School.Common.OtherAPIClient.Comment.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.Common.OtherAPIClient.Comment
{
    public interface ICommentAPIClient
    {
        Task<IEnumerable<GetQuestionByEIDResponse>> GetQuestionByEID(Guid eid, Guid userID,int take = 1);
        Task<IEnumerable<SchoolCommentDTO>> GetSchoolSelectedComment(Guid eid, Guid? userID, int take = 1);
        Task<IEnumerable<SchoolCommentTotalDTO>> GetCommentTotals(Guid sid);
        /// <summary>
        /// 获取问答统计
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<IEnumerable<SchoolQuestionTotalDTO>> GetQuestionTotals(Guid eid);

        /// <summary>
        /// 获取学校评分(家长评价)共n人点评
        /// </summary>
        /// <param name="eids"></param>
        /// <returns></returns>
        Task<IEnumerable<SchoolScoreCommentCountDto>> GetSchoolScoreCommentCountByEids(IEnumerable<Guid> eids);
    }
}
