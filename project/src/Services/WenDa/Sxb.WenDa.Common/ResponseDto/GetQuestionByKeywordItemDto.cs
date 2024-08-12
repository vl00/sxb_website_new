using System;
using System.Collections.Generic;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class GetQuestionByKeywordItemDto
    {
        /// <summary>问题id</summary>
        public Guid QuestionId { get; set; }
        /// <summary>问题短id</summary>
        public string QuestionNo { get; set; }
        /// <summary>问题标题</summary>
        public string Title { get; set; }
        /// <summary>回答数</summary>
        public int ReplyCount { get; set; }
    }
}
