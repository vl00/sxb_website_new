using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 用户订阅学部表
    /// </summary>
    public class ExtensionSubscribeInfo
    {
        [Identity(false)]
        public Guid ID { get; set; }
        public Guid EID { get; set; }
        public Guid UserID { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
