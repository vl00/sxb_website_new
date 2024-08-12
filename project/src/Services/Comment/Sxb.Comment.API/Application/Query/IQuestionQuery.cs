using Sxb.Comment.Common.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.Comment.API.Application.Query
{
    public interface IQuestionQuery
    {
        Task<IEnumerable<QuestionDTO>> ListByEIDs(IEnumerable<Guid> eids, Guid userID,int take = 1);
        Task<IEnumerable<SchoolQuestionTotalDTO>> ListTotalsByEID(Guid eid);
    }
}
