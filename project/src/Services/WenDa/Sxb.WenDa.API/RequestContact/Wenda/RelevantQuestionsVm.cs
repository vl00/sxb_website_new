using Sxb.WenDa.Common.ResponseDto;
using System;
using System.Collections.Generic;

namespace Sxb.WenDa.API.RequestContact.Wenda
{
    public class RelevantQuestionsQuery
    {
        /// <summary>城市编码</summary>
        public long? City { get; set; }
        /// <summary>分类ids</summary>
        public long[] CategoryIds { get; set; }
        /// <summary>标签ids</summary>
        public long[] TagIds { get; set; }

        /// <summary>不包含的问题长id</summary>
        public Guid? QidNotIn { get; set; }
    }

    /// <summary>
    /// 相关问题s
    /// </summary>
    public class RelevantQuestionsVm
    {
        /// <summary>相关问题s</summary>
        public RelevantQuestionDto[] RelevantQuestions { get; set; } // <=6
    }
}
