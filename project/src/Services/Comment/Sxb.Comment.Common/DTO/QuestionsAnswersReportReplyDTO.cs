using Sxb.Comment.Common.Entity;
using System;

namespace Sxb.Comment.Common.DTO
{
    public class QuestionsAnswersReportReplyDTO
    {
        public Guid ID { get; set; }

        public Guid ReportID { get; set; }

        public Guid AdminID { get; set; }

        public string ReplyContent { get; set; }

        public DateTime CreateTime { get; set; }

        public virtual QuestionsAnswersReportInfo QuestionsAnswersReport { get; set; }
    }
}
