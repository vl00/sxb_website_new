using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.Comment.Common.Entity
{
    [Display(Rename = "QuestionsAnswerExamines")]
    public class QuestionsAnswerExamineInfo
    {
        [Identity]
        public Guid ID { get; set; }
        /// <summary>
        /// 问题ID
        /// </summary>
        public Guid QuestionsAnswersInfoID { get; set; }
        /// <summary>
        /// 审核者
        /// </summary>
        public Guid AdminID { get; set; }
        public bool IsPartTimeJob { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ExamineTime { get; set; }
        public virtual QuestionsAnswersInfo QuestionsAnswersInfo { get; set; }
    }
}
