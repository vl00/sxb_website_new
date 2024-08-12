using Newtonsoft.Json;
using Sxb.Framework.Foundation;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class SchoolQuestionLinkDto
    {
        public Guid Id { get; set; }

        public Guid ExtId { get; set; }

        /// <summary>
        /// 问题短链
        /// </summary>
        [JsonIgnore]
        public long No { get; set; }

        public string QuestionNo => UrlShortIdUtil.Long2Base32(No);

        /// <summary>
        /// 问题标题
        /// </summary>
        [JsonIgnore]
        public string Title { get; set; }
        public string QuestionTitle => Title;
    }
}
