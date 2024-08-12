using Kogel.Dapper.Extension.Attributes;
using Sxb.Comment.Common.Enum;
using System;
using System.Collections.Generic;

namespace Sxb.Comment.Common.Entity
{
    public class QuestionInfo
    {
        public QuestionInfo()
        {
            ID = Guid.NewGuid();
            State = ExamineStatus.Unread;
            IsTop = false;
            LikeCount = 0;
            ReplyCount = 0;
            IsHaveImagers = false;
            IsAnony = false;
        }

        [Identity]
        public Guid ID { get; set; }
        /// <summary>
        /// 问题审核状态（0：未阅，1：已阅，2：已加精，4：已屏蔽） 默认值为：0（可以在前端显示）
        /// </summary>
        public ExamineStatus State { get; set; }
        /// <summary>
        /// 学校ID
        /// </summary>
        public Guid SchoolID { get; set; }
        /// <summary>
        /// 学校学部ID
        /// </summary>
        public Guid SchoolSectionID { get; set; }
        /// <summary>
        /// 问题写入者
        /// </summary>
        public Guid UserID { get; set; }

        public UserRoleType PostUserRole { get; set; }
        /// <summary>
        /// 问题内容
        /// </summary>
        public string Content { get; set; }
        //点赞数
        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否有上传图片
        /// </summary>
        public bool IsHaveImagers { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public long No { get; set; }
        /// <summary>
        /// 浏览量
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }
        public virtual QuestionExaminesInfo QuestionExamine { get; set; }

        public virtual List<QuestionsAnswersInfo> QuestionsAnswersInfos { get; set; }
        public virtual List<QuestionsAnswersReportInfo> QuestionsAnswersReports { get; set; }
    }
}
