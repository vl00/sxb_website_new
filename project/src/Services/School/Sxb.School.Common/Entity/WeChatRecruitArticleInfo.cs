using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;
using Sxb.Framework.Foundation;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 微信搜索政策大卡-招生政策-文章
    /// </summary>
    public class WeChatRecruitArticleInfo
    {
        [Identity(false)]
        public Guid ID { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonIgnore]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 副标题
        /// </summary>
        [JsonIgnore]
        public string SubTitle { get; set; }
        [Display(IsField = false)]
        public IEnumerable<WeChatRecruitArticleSubTitleItem> SubTitle_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SubTitle)) return SubTitle.FromJsonSafe<IEnumerable<WeChatRecruitArticleSubTitleItem>>();
                return default;
            }
        }
        /// <summary>
        /// 来源文字内容
        /// </summary>
        public string SourceText { get; set; }
        /// <summary>
        /// 来源跳转Url
        /// </summary>
        public string SourceUrl { get; set; }
    }
    public class WeChatRecruitArticleSubTitleItem
    {
        public string Content { get; set; }
        public SubTitleTextAlignType Align { get; set; }
        public Guid? ID { get; set; } = Guid.NewGuid();
    }
}
