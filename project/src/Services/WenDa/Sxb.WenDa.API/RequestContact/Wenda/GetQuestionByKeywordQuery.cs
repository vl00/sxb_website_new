using Sxb.WenDa.Common.ResponseDto;
using System;
using System.Collections.Generic;

namespace Sxb.WenDa.API.RequestContact.Wenda
{
    /// <summary>
    /// 最多显示8个好了
    /// </summary>
    public class GetQuestionByKeywordQuery
    {
        public string Keyword { get; set; }
    }

    public class GetQuestionByKeywordQueryResult
    {
        public List<GetQuestionByKeywordItemDto> Items { get; set; }
    }
}
