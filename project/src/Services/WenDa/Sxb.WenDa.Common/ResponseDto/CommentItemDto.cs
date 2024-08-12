using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class CommentItemDto
    {
        /// <summary>评论id</summary>
        public Guid Id { get; set; }

        /// <summary>用户id</summary>
        public Guid UserId { get; set; }
        /// <summary>用户名</summary>
        public string UserName { get; set; }
        /// <summary>用户头像</summary>
        public string UserHeadImg { get; set; }

        /// <summary>
        /// 回复谁,谁的用户id.<br/>
        /// 发评论回复回答时,为null
        /// </summary>
        public Guid? FromUserId { get; set; }
        /// <summary>
        /// 回复谁,谁的用户名.<br/>
        /// 发评论回复回答时,为null
        /// </summary>
        public string FromUserName { get; set; } = null;

        /// <summary>评论内容</summary>
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToTimestampJsonConverter))]
        public DateTime? CreateTime { get; set; } = default!;
        /// <summary>
        /// 最后一次编辑时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToTimestampJsonConverter))]
        public DateTime? EditTime { get; set; } = null;

        /// <summary>点赞数</summary>
        public int LikeCount { get; set; } = 0;
        /// <summary>true=是我点赞的</summary>
        public bool? IsLikeByMe { get; set; }

        /// <summary>子评论数</summary>
        public int? ReplyCount { get; set; }

        /// <summary>
        /// 子评论s <br/>
        /// 1. 目前为前端展开1级评论后显示最多2条2级子评论.展开其他回复数=1级里ReplyCount - 此数组长度.<br/>
        /// 2. 2级子评论里此字段为null
        /// </summary>
        public CommentItemDto[] Children { get; set; } = null; // <=2
    }
}
