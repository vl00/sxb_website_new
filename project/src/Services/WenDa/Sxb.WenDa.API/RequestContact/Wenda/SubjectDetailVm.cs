using Sxb.WenDa.Common.ResponseDto;
using System;
using System.Collections.Generic;

namespace Sxb.WenDa.API.RequestContact.Wenda
{
    public class SubjectDetailVm
    {
        /// <summary>专栏id</summary>
        public Guid SubjectId => Subject.SubjectId;
        /// <summary>专栏短id</summary>
        public string SubjectNo => Subject.SubjectNo;

        /// <summary>专栏dto</summary>
        public SubjectItemDto Subject { get; set; }

        /// <summary>城市入口</summary>
        public CityDto[] Cities { get; set; }

        /// <summary>相关问题s</summary>
        public RelevantQuestionDto[] RelevantQuestions { get; set; } // <=6

        /// <summary>相关推荐s</summary>
        public RelevantArticleDto[] RecommendArticles { get; set; } = null; // 查文章 <=6

        
    }
}
