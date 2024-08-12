using Sxb.Comment.Common.Enum;
using System;

namespace Sxb.Comment.Common.DTO
{
    public class SchoolQuestionTotalDTO
    {
        /// <summary>
        /// 统计类型
        /// </summary>
        public QuestionTotalType TotalType { get; set; }
        /// <summary>
        /// 统计总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 分部id
        /// </summary>
        public Guid? SchoolSectionID { get; set; }
    }
}
