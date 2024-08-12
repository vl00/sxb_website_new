using Kogel.Dapper.Extension.Attributes;
using Sxb.Comment.Common.DTO;
using Sxb.Comment.Common.Enum;
using System;

namespace Sxb.Comment.Common.Entity
{
    public class QuestionsAnswersReportInfo
    {
        public QuestionsAnswersReportInfo()
        {
            ID = Guid.NewGuid();
        }

        [Identity]
        public Guid ID { get; set; }
        /// <summary>
        /// 举报原因
        /// </summary>
        /// <value>The type of the report reason.</value>
        public int ReportReasonType { get; set; }
        /// <summary>
        /// 举报人ID
        /// </summary>
        /// <value>The report user IDentifier.</value>
        public Guid ReportUserID { get; set; }
        /// <summary>
        /// 问题ID
        /// </summary>
        public Guid QuestionID { get; set; }
        /// <summary>
        /// 回答ID
        /// </summary>
        public Guid? QuestionsAnswersInfoID { get; set; }
        /// <summary>
        /// 回复ID
        /// </summary>
        public Guid? AnswersReplyID { get; set; }
        /// <summary>
        /// 举报内容
        /// </summary>
        public string ReportDetail { get; set; }
        /// <summary>
        /// 举报类型 
        /// </summary>
        public ReportDataType ReportDataType { get; set; }
        /// <summary>
        /// 举报内容 0、未回复 1、已回复
        /// </summary>
        public ReportStatus Status { get; set; }
        /// <summary>
        /// 举报时间
        /// </summary>
        public DateTime ReportTime { get; set; }
        public virtual QuestionInfo QuestionInfos { get; set; }
        public virtual ReportTypeInfo ReportType { get; set; }
        public virtual QuestionsAnswersReportReplyDTO QuestionsAnswersReportReply { get; set; }
    }
}
