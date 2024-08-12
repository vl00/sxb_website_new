﻿using System;

namespace Sxb.Comment.Common.DTO
{
    public class AnswerInfoDTO
    {
        /// <summary>
        /// 回答ID
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 该回答的点赞数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 回答的回复数
        /// </summary>
        public int ReplyCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Reply { get; set; }
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 当前回复是否为精华
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// 回答内容
        /// </summary>
        public string AnswerContent { get; set; }
        public long AddTime { get; set; }

        /// <summary>
        /// 是否学校发布
        /// </summary>
        public bool IsSchoolPublish { get; set; }
        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }

        /// <summary>
        /// 问题ID
        /// </summary>
        public Guid QuestionID { get; set; }
        /// <summary>
        /// 学校ID
        /// </summary>
        public Guid SchoolID { get; set; }
        /// <summary>
        /// 学校分部ID
        /// </summary>
        public Guid SchoolSectionID { get; set; }

        public Guid UserID { get; set; }
        public Guid? ParentID { get; set; }
        public Guid? ParentUserID { get; set; }
        public bool ParentUserIDIsAnony { get; set; }
        public bool IsTalent { get; set; }
        public int? TalentType { get; set; }
        public string NickName { get; set; }
        public string HeadImgUrl { get; set; }

        public QuestionDTO Question { get; set; }
    }
}
