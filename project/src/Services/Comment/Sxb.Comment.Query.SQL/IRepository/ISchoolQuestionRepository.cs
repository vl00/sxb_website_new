using Sxb.Comment.Common.DTO;
using Sxb.Comment.Common.Entity;
using Sxb.Comment.Common.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.Comment.Query.SQL.IRepository
{
    public interface ISchoolQuestionRepository
    {
        Task<IEnumerable<QuestionInfo>> GetSchoolSelectedQuestion(IEnumerable<Guid> schoolSectionIDs, SelectedQuestionOrderType order, int take = 1);
        QuestionDTO ConvertToQuestionDTO(QuestionInfo questionInfo, Guid userID, IEnumerable<GiveLikeInfo> likes, IEnumerable<SchoolImageInfo> images, IEnumerable<AnswerInfoDTO> answers = null);
        Task<IEnumerable<SchoolQuestionTotalDTO>> GetQuestionTotalsByEID(Guid eid);
    }
}
