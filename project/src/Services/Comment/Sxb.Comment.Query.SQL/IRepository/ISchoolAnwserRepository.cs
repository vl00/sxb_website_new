using Sxb.Comment.Common.DTO;
using Sxb.Comment.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.Comment.Query.SQL.IRepository
{
    public interface ISchoolAnwserRepository
    {
        Task<IEnumerable<AnswerInfoDTO>> QuestionAnswersOrderByQuestionIDs(IEnumerable<QuestionInfo> questions, Guid userID, int take);
        Task<IEnumerable<QuestionsAnswersInfo>> QuestionAnswersOrderByRole(IEnumerable<Guid> questionInfoIDs, int take);
        AnswerInfoDTO ConvertToAnswerInfoDTO(QuestionsAnswersInfo questionsAnswers, IEnumerable<QuestionInfo> questions, IEnumerable<GiveLikeInfo> likes);
    }
}
