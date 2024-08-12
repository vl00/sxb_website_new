using Sxb.School.Common.OtherAPIClient.Comment.Model.Enum;
using System;
using System.Collections.Generic;

namespace Sxb.School.Common.OtherAPIClient.Comment.Models.Entity
{
    /// <summary>
    /// 问题及回复详情展示
    /// </summary>
    public class QuestionDTO
    {
        /// <summary>
        /// 序号
        /// </summary>
        public long No { get; set; }
        /// <summary>
        /// 问题ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 学校ID
        /// </summary>
        public Guid SchoolID { get; set; }

        /// <summary>
        /// 学校分部ID
        /// </summary>
        public Guid SchoolSectionID { get; set; }

        public Guid UserID { get; set; }
        /// <summary>
        /// 问题内容
        /// </summary>
        public string QuestionContent { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int AnswerCount { get; set; }
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 当前用户是否回复
        /// </summary>
        public bool IsAnswer { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 问题图片
        /// </summary>
        public List<string> Images { get; set; }
        public DateTime QuestionCreateTime { get; set; }
        /// <summary>
        /// 当前问题的回答详情
        /// </summary>
        public List<AnswerInfoDTO> Answer { get; set; }
        /// <summary>
        /// 是否为精选
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// 是否存在问题
        /// </summary>
        public bool IsExists { get; set; }

        public ExamineStatus State { get; set; }
        public int? TalentType { get; set; }

        /// <summary>
        /// 当前学校下的总问题数
        /// </summary>
        public int SchoolQuestionTotal { get; set; }
        /// <summary>
        /// 学校总回复数
        /// </summary>
        public int SchoolReplyTotal { get; set; }

    }
}
