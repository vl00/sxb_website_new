using Sxb.School.Common.Enum;
using System;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 纠错留言
    /// </summary>
    public class CorrectionMessageInfo
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 身份类型
        /// </summary>
        public IdentityType IdentityType { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
