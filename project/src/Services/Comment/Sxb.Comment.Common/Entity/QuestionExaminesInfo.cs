using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.Comment.Common.Entity
{
    [Display(Rename = "QuestionExamines")]
    public class QuestionExaminesInfo
    {
        [Identity]
        public Guid ID { get; set; }
        /// <summary>
        /// 审核者
        /// </summary>
        public Guid AdminID { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime ExamineTime { get; set; }
        /// <summary>
        /// 问题
        /// </summary>
        public Guid QuestionInfoID { get; set; }

        public virtual QuestionInfo QuestionInfo { get; set; }
    }
}
