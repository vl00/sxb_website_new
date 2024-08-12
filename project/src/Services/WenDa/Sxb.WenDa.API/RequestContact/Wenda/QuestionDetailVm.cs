using Sxb.WenDa.Common.ResponseDto;
using System;
using System.Collections.Generic;

namespace Sxb.WenDa.API.RequestContact.Wenda
{
    /// <summary>
    /// 问答详情
    /// </summary>
    public class QuestionDetailVm
    {
        /// <summary>问题id</summary>
        public Guid QuestionId => Question?.QuestionId ?? default;
        /// <summary>问题短id</summary>
        public string QuestionNo => Question?.QuestionNo;

        /// <summary>问题dto</summary>
        public QaQuestionItemDto Question { get; set; }

        /// <summary>城市入口</summary>
        public CityDto[] Cities { get; set; }

        /// <summary>相关问题s</summary>
        public RelevantQuestionDto[] RelevantQuestions { get; set; } // <=6
        /// <summary>相关推荐s</summary>
        public RelevantQuestionDto[] RecommendQuestions { get; set; } // <=6

        ///// <summary>回答s</summary>
        //public QaAnswerItemDto[] Answers { get; set; }

        /// <summary>分享的回答</summary>
        public QaAnswerItemDto SharedAnswer { get; set; }


    }
}
