using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto.Home
{
    public class HotSubjectItemDto
    {
        /// <summary>
        /// 专栏id
        /// </summary>
        public Guid SubjectId { get; set; }

        /// <summary>
        /// 短id
        /// </summary>
        public string SubjectNo { get; set; }

        /// <summary>
        /// 专栏名称
        /// </summary>
        public string SubjectTitle { get; set; }

        /// <summary>
        /// 专栏封面图
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 推荐的问题列表
        /// </summary>
        public IEnumerable<QuestionLinkDto> Questions { get; set; }
    }
}
