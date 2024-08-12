using Newtonsoft.Json;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class QuestionCategoryItemDto
    {
        public QuestionCategoryItemDto()
        {
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
        /// 问题分类
        /// </summary>
        public string CategoryName { get; set; }
    }
}
