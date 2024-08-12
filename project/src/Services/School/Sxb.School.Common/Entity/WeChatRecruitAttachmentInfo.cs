using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;
using System;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 微信搜索政策大卡-招生政策-附件
    /// </summary>
    public class WeChatRecruitAttachmentInfo
    {
        [Identity(false)]
        [JsonIgnore]
        public Guid ID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 附件地址
        /// </summary>
        public string Url { get; set; }
    }
}