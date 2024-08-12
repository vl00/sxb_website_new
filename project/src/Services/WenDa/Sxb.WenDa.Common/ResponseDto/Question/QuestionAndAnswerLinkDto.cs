using Newtonsoft.Json;
using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.Entity;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class QuestionAndAnswerLinkDto
    {
        public QuestionAndAnswerLinkDto() { }
        public QuestionAndAnswerLinkDto(QuestionLinkDto questionLink)
        {
            Id = questionLink.Id;
            QuestionNo = questionLink.QuestionNo;
            QuestionTitle = questionLink.QuestionTitle;
        }
        public QuestionAndAnswerLinkDto(Question question)
        {
            Id = question.Id;
            QuestionNo = UrlShortIdUtil.Long2Base32(question.No);
            QuestionTitle = question.Title;
        }

        public QuestionAndAnswerLinkDto SetAnswer(QuestionAnswer answer)
        {
            if (answer != null)
            {
                AnswerNo = UrlShortIdUtil.Long2Base32(answer.No);
                AnswerContent = answer.Content;
            }
            return this;
        }

        [JsonIgnore]
        public Guid Id { get; set; }

        /// <summary>
        /// 问题短链
        /// </summary>
        public string QuestionNo { get; set; }

        /// <summary>
        /// 问题标题
        /// </summary>
        public string QuestionTitle { get; set; }

        /// <summary>
        /// 回答链接
        /// </summary>
        public string AnswerNo { get; set; }

        /// <summary>
        /// 回答内容
        /// </summary>
        public string AnswerContent { get; set; }
    }
}
