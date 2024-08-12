using Newtonsoft.Json;
using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class WaitQuestionItemDto
    {
        public WaitQuestionItemDto()
        {
            User = new UserDto();
        }

        public WaitQuestionItemDto(Question question)
        {
            Id = question.Id;
            QuestionNo = UrlShortIdUtil.Long2Base32(question.No);
            QuestionTitle = question.Title;
            QuestionTime = question.CreateTime;
            User = new UserDto()
            {
                UserId = question.UserId
            };
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
        /// 提问时间
        /// </summary>
        public DateTime QuestionTime { get; set; }

        /// <summary>
        /// 提问人
        /// </summary>
        public UserDto User { get; }
    }
}
