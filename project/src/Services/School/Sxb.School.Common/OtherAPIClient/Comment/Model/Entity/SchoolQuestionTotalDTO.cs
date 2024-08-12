using System;

namespace Sxb.School.Common.OtherAPIClient.Comment.Model.Entity
{
    public class SchoolQuestionTotalDTO
    {
        /// <summary>
        /// 统计类型
        /// </summary>
        public string TotalTypeName { get; set; }
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
