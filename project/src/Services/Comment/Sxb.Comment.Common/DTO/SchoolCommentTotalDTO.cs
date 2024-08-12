using Sxb.Comment.Common.Enum;
using System;

namespace Sxb.Comment.Common.DTO
{
    /// <summary>
    /// 学校点评统计
    /// </summary>
    public class SchoolCommentTotalDTO
    {
        /// <summary>
        /// 统计类型
        /// </summary>
        public QueryConditionType TotalType { get; set; }
        /// <summary>
        /// 统计总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 分部id
        /// </summary>
        public Guid? SchoolSectionID { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string Name { get; set; }
    }
}
