using Sxb.Comment.Common.DTO;
using System;
using System.Collections.Generic;

namespace Sxb.Comment.API.RequestContract.Comment
{
    /// <summary>
    /// 获取学校评分(家长评价)共n人点评
    /// </summary>
    public class GetSchoolScoreCommentCountByEidsRequest
    {
        /// <summary>
        /// 学部eids
        /// </summary>
        public Guid[] Eids { get; set; }
    }

    public class GetSchoolScoreCommentCountByEidsResponse
    {
        public IEnumerable<SchoolScoreCommentCountDto> Items { get; set; }
    }

    
}
