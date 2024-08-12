using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.RequestDto
{
    public class GetCommentsPageListQuery
    {
        /// <summary>
        /// 回答id,用于查1级评论 <br/>
        /// answerId 与 commentId 只能传其中一个
        /// </summary>
        public Guid? AnswerId { get; set; }
        /// <summary>
        /// 评论id,用于查2级评论 <br/>
        /// answerId 与 commentId 只能传其中一个
        /// </summary>
        public Guid? CommentId { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// 排序 1=点赞多到少 2=时间近到远 <br/>
        /// 一级评论排序按点赞 二级按时间从近到远
        /// </summary>
        public int Orderby { get; set; }

        /// <summary>
        /// 后端字段,前端不管
        /// </summary>
        [JsonIgnore]
        public Guid UserId { get; set; }

        /// <summary>
        /// 时间戳,可选. 表示返回不查询此时间之后的数据
        /// </summary>
        [JsonConverter(typeof(DateTimeToTimestampJsonConverter))]
        public DateTime? Naf { get; set; }
    }
}
