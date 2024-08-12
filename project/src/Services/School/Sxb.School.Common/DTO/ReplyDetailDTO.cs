using System;

namespace Sxb.School.Common.DTO
{
    public class ReplyDetailDTO
    {
        public Guid Id { get; set; }

        public Guid DatascoureId { get; set; }

        public UserInfoDTO UserInfo { get; set; }

        /// <summary>
        /// 学校学部ID
        /// </summary>
        public Guid SchoolSectionId { get; set; }

        /// <summary>
        /// 是否已点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 是否学校发布
        /// </summary>
        public bool IsSchoolPublish { get; set; }
        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; }

        public int ReplyCount { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }

        public string CreateTime { get; set; }
    }
}
