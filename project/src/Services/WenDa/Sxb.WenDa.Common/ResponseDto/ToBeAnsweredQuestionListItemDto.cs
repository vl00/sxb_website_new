using Newtonsoft.Json;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    /// <summary>
    /// 待回答列表item项
    /// </summary>
    public class ToBeAnsweredQuestionListItemDto : QaQuestionListItemDto
    {
        /// <summary>待回答列表,此字段为null</summary>
        public new QaAnswerItemDto Answer
        {
            get => base.Answer = null;
            set => base.Answer = null;
        }
    }
}
