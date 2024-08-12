using Sxb.Comment.Common.DTO;
using Sxb.Comment.Common.Entity;
using Sxb.Comment.Common.Enum;
using Sxb.Comment.Common.OtherAPIClient.User;
using Sxb.Comment.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Comment.API.Application.Query
{
    public class QuestionQuery : IQuestionQuery
    {
        readonly ISchoolQuestionRepository _schoolQuestionRepository;
        readonly ISchoolAnwserRepository _schoolAnwserRepository;
        readonly ISchoolGiveLikeRepository _schoolGiveLikeRepository;
        readonly ISchoolImageRepository _schoolImageRepository;
        readonly IUserAPIClient _userAPIClient;

        public QuestionQuery(ISchoolQuestionRepository schoolQuestionRepository, ISchoolAnwserRepository schoolAnwserRepository, ISchoolGiveLikeRepository schoolGiveLikeRepository, ISchoolImageRepository schoolImageRepository, IUserAPIClient userAPIClient)
        {
            _schoolQuestionRepository = schoolQuestionRepository;
            _schoolAnwserRepository = schoolAnwserRepository;
            _schoolGiveLikeRepository = schoolGiveLikeRepository;
            _schoolImageRepository = schoolImageRepository;
            _userAPIClient = userAPIClient;
        }

        public async Task<IEnumerable<QuestionDTO>> ListByEIDs(IEnumerable<Guid> eids, Guid userID, int take = 1)
        {
            if (eids == null || !eids.Any()) return null;
            var questionDTOs = new List<QuestionDTO>();
            var schoolSectionQuestions = await _schoolQuestionRepository.GetSchoolSelectedQuestion(eids, SelectedQuestionOrderType.None, take);
            var answerDTOs = new List<AnswerInfoDTO>();
            if (schoolSectionQuestions?.Any() == true)
            {
                var questionIDs = schoolSectionQuestions.Select(p => p.ID).Distinct();
                var questionAnswers = await _schoolAnwserRepository.QuestionAnswersOrderByRole(questionIDs, 1);
                if (questionAnswers?.Any() == true)
                {
                    var userInfosTask = _userAPIClient.ListTalentDetails(questionAnswers.Where(p => p.UserID != default).Select(p => p.UserID).Distinct());
                    var likes = await _schoolGiveLikeRepository.CheckCurrentUserIsLikeBulk(userID, questionAnswers.Select(p => p.ID).Distinct());
                    var answers = questionAnswers.Select(x => _schoolAnwserRepository.ConvertToAnswerInfoDTO(x, schoolSectionQuestions, likes));
                    var userInfos = await userInfosTask;
                    if (userInfos?.Any() == true && answers?.Any() == true)
                    {
                        foreach (var item in answers.Where(p => p.IsAnony == false))
                        {
                            var userInfo = userInfos.FirstOrDefault(p => p.ID == item.UserID);
                            if (userInfo?.ID != default)
                            {
                                item.IsTalent = userInfo.IsAuth;
                                item.TalentType = userInfo.Role;
                                item.HeadImgUrl = userInfo.HeadImgUrl;
                                item.NickName = userInfo.Nickname;
                            }
                            answerDTOs.Add(item);
                        }
                    }
                }
                foreach (var question in schoolSectionQuestions)
                {
                    var likes = question.UserID == default ? new List<GiveLikeInfo>() : await _schoolGiveLikeRepository.CheckCurrentUserIsLikeBulk(question.UserID, new List<Guid>() { question.ID });
                    var images = await _schoolImageRepository.GetImageByDataSourceIDs(new List<Guid>() { question.ID }, ImageType.Question);

                    questionDTOs.Add(_schoolQuestionRepository.ConvertToQuestionDTO(question, question.UserID, likes, images, answerDTOs));
                }
            }

            return questionDTOs;
        }

        public async Task<IEnumerable<SchoolQuestionTotalDTO>> ListTotalsByEID(Guid eid)
        {
            return await _schoolQuestionRepository.GetQuestionTotalsByEID(eid);
        }
    }
}
