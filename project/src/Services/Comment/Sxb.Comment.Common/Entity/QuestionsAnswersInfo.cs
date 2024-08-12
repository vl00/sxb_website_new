using Kogel.Dapper.Extension.Attributes;
using Sxb.Comment.Common.Enum;
using System;

namespace Sxb.Comment.Common.Entity
{
    [Display(Rename = "QuestionsAnswersInfos")]
    public class QuestionsAnswersInfo
    {
        public QuestionsAnswersInfo()
        {
            ID = Guid.NewGuid();
            State = 0;
            IsTop = false;
            LikeCount = 0;
            ReplyCount = 0;
            IsSettlement = false;
        }

        [Identity]
        public Guid ID { get; set; }
        /// <summary>
        /// 问答ID
        /// </summary>
        public Guid QuestionInfoID { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentID { get; set; }

        /// <summary>
        /// 问题审核状态（1：未阅，2：已阅，3：已加精，4：已屏蔽） 默认值为：1（可以在前端显示）
        /// </summary>
        public ExamineStatus State { get; set; }

        public bool IsTop { get; set; }

        /// <summary>
        /// 问答写入者
        /// </summary>
        public Guid UserID { get; set; }

        public UserRoleType PostUserRole { get; set; }

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
        /// 问答内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点赞总数
        /// </summary>

        public int LikeCount { get; set; }
        /// <summary>
        /// 回复总数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 浏览量
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 是否已结算
        /// </summary>
        public bool IsSettlement { get; set; }

        /// <summary>
        /// 问答写入日期
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 顶级父级ID
        /// </summary>
        public Guid? FirstParentID { get; set; }

        public virtual QuestionsAnswerExamineInfo QuestionsAnswerExamine { get; set; }

        public virtual QuestionInfo QuestionInfo { get; set; }
        public virtual QuestionsAnswersInfo ParentAnswerInfo { get; set; }
    }
}
