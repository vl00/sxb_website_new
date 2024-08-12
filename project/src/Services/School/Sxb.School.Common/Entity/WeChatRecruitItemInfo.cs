using Kogel.Dapper.Extension.Attributes;
using Sxb.School.Common.Enum;
using System;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 微信搜索政策大卡-招生政策-子项
    /// </summary>
    public class WeChatRecruitItemInfo
    {
        [Identity(false)]
        public Guid ID { get; set; }
        /// <summary>
        /// 招生政策ID
        /// </summary>
        public Guid RecruitID { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public WeChatRecruitItemType Type { get; set; }
        /// <summary>
        /// 子项ID
        /// </summary>
        public Guid ItemID { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }
    }
}
