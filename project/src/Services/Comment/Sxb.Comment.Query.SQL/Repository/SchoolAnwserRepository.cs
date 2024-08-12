using Dapper;
using Sxb.Comment.Common.DB;
using Sxb.Comment.Common.DTO;
using Sxb.Comment.Common.Entity;
using Sxb.Comment.Common.Enum;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Comment.Query.SQL.IRepository
{
    public class SchoolAnwserRepository : ISchoolAnwserRepository
    {
        readonly SchoolProductDB _schoolProductDB;
        readonly ISchoolGiveLikeRepository _schoolGiveLikeRepository;

        public SchoolAnwserRepository(SchoolProductDB schoolProductDB, ISchoolGiveLikeRepository schoolGiveLikeRepository)
        {
            _schoolProductDB = schoolProductDB;
            _schoolGiveLikeRepository = schoolGiveLikeRepository;
        }

        public AnswerInfoDTO ConvertToAnswerInfoDTO(QuestionsAnswersInfo questionsAnswers, IEnumerable<QuestionInfo> questions, IEnumerable<GiveLikeInfo> likes)
        {
            if (questionsAnswers == null)
            {
                return null;
            }
            var question = questions.Where(x => x.ID == questionsAnswers.QuestionInfoID).FirstOrDefault();
            return new AnswerInfoDTO()
            {
                ID = questionsAnswers.ID,
                UserID = questionsAnswers.UserID,
                IsLike = likes != null ? likes.FirstOrDefault(q => q.SourceID == questionsAnswers.ID) != null : false,
                AnswerContent = questionsAnswers.Content,
                LikeCount = questionsAnswers.LikeCount,
                IsSelected = questionsAnswers.State == ExamineStatus.Highlight,
                AddTime = questionsAnswers.CreateTime.ToUnixTimestampByMilliseconds(),
                ReplyCount = questionsAnswers.ReplyCount,
                IsAnony = questionsAnswers.IsAnony,
                IsAttend = questionsAnswers.IsAttend,
                IsSchoolPublish = questionsAnswers.IsSchoolPublish,
                ParentID = questionsAnswers.ParentID,
                QuestionID = questionsAnswers.QuestionInfoID,
                SchoolID = question != null ? question.SchoolID : default(Guid),
                SchoolSectionID = question != null ? question.SchoolSectionID : default(Guid),
                Question = question != null ? new QuestionDTO()
                {
                    ID = question.ID,
                    QuestionContent = question.Content,
                    SchoolSectionID = question.SchoolSectionID,
                    No = question.No
                } : new QuestionDTO()
            };
        }

        public async Task<IEnumerable<AnswerInfoDTO>> QuestionAnswersOrderByQuestionIDs(IEnumerable<QuestionInfo> questions, Guid userID, int take)
        {
            if (questions == null || !questions.Any()) return null;
            var finds = await QuestionAnswersOrderByRole(questions.Select(p => p.ID).Distinct(), take);
            if (finds == null || !finds.Any()) return null;
            var likes = await _schoolGiveLikeRepository.CheckCurrentUserIsLikeBulk(userID, finds.Select(p => p.ID).Distinct());
            return finds.Select(x => ConvertToAnswerInfoDTO(x, questions, likes))?.ToList();
        }

        public async Task<IEnumerable<QuestionsAnswersInfo>> QuestionAnswersOrderByRole(IEnumerable<Guid> questionInfoIDs, int take)
        {
            string str_SQL = @$"SELECT TOP {take}
			                        *,
		                        CASE
				                        WHEN PostUserRole = '1' THEN
				                        2 
				                        WHEN PostUserRole = '2' THEN
				                        1 ELSE 0 
			                        END AS UserRole
		                        FROM
			                        QuestionsAnswersInfos 
		                        WHERE
			                        QuestionInfoID IN @questionInfoIDs
                                    AND ParentID is null
			                        AND State in (0,1,2,3)
                                Order By IsTop, UserRole DESC, LikeCount DESC";
            return await _schoolProductDB.SlaveConnection.QueryAsync<QuestionsAnswersInfo>(str_SQL, new { questionInfoIDs, take });
        }
    }
}
