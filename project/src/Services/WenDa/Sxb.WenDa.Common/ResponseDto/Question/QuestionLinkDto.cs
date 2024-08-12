using Newtonsoft.Json;
using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.Entity;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class QuestionLinkDto
    {
        public QuestionLinkDto()
        {
        }

        public QuestionLinkDto(Question question)
        {
            Id = question.Id;
            QuestionNo = UrlShortIdUtil.Long2Base32(question.No);
            QuestionTitle = question.Title;
        }

        public QuestionLinkDto(SchoolQuestionLinkDto question)
        {
            Id = question.Id;
            QuestionNo = question.QuestionNo;
            QuestionTitle = question.QuestionTitle;
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
    }
}
