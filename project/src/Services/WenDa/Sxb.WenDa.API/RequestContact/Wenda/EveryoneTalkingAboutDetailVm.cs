using Sxb.WenDa.Common.ResponseDto;
using System;
using System.Collections.Generic;

namespace Sxb.WenDa.API.RequestContact.Wenda
{
    /// <summary>
    /// 大家热议
    /// </summary>
    public class EveryoneTalkingAboutDetailVm
    {
        /// <summary>城市入口</summary>
        public CityDto[] Cities { get; set; }

        /// <summary>热门推荐</summary>
        public RelevantQuestionDto[] HotRecommends { get; set; } // <=8

    }
}
