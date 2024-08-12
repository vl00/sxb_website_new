using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.RequestDto
{
    public class GetQuestionsPageListBySubjectQuery
    {
        /// <summary>
        /// 专栏id(长短都行)
        /// </summary>
        public string SubjectId { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// 排序 <br/>
        /// 1=热门问题 2=最新提问
        /// </summary>
        public int Orderby { get; set; } = 1;

        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
