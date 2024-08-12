using Newtonsoft.Json;
using Sxb.Comment.Common.Entity;
using Sxb.Comment.Common.Enum;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;

namespace Sxb.Comment.Common.DTO
{
    public class SchoolCommentDTO
    {
        public long No { get; set; }
        public string ShortCommentNo
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(No)?.ToLower();
            }
        }
        /// <summary>
        /// 点评id
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 写入点评的用户id
        /// </summary>
        [JsonIgnore]
        public Guid UserID { get; set; }
        public string HeadImgUrl { get; set; }
        public string NickName { get; set; }
        public bool IsTalent { get; set; }
        /// <summary>
        /// 学校id 
        /// </summary>
        public Guid SchoolID { get; set; }
        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid SchoolSectionID { get; set; }
        /// <summary>
        /// 写入点评时勾选、新建的标签
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
        /// <summary>
        /// 写点评上传的图片集合，也需要取配置文件中的域名
        /// </summary>
        public IEnumerable<string> Images { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }
        public ExamineStatus State { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }
        /// <summary>
        /// 点评评分
        /// </summary>
        public SchoolCommentScoreInfo Score { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }
        //点赞数
        public int LikeCount { get; set; }
        /// <summary>
        /// 点亮
        /// </summary>
        public int StartTotal { get; set; }
        /// <summary>
        /// 是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 是否为精选
        /// </summary>
        public bool IsSelected => State == ExamineStatus.Highlight || ReplyCount >= 10;
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool IsRumorRefuting { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }

        public IEnumerable<CommentReplyDTO> CommentReplies { get; set; }

        public int? TalentType { get; set; }
    }
}
