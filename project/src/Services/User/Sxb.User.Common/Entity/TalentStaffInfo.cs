using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.User.Common.Entity
{
    [Display(Rename = "talent_staff")]
    public class TalentStaffInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [Identity]
        public Guid ID { get; set; }
        /// <summary>
        /// 员工达人ID
        /// </summary>
        public Guid Talent_ID { get; set; }
        /// <summary>
        /// 员工归属的机构达人ID
        /// </summary>
        public Guid ParentID { get; set; }
        /// <summary>
        /// 员工名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 员工职位
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 员工电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 状态 0:已邀请 ，1:已接受
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 删除标记 0否，1是
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Createdate { get; set; }
    }
}
