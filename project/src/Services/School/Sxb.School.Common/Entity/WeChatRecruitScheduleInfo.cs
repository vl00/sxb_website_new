using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;

namespace Sxb.School.Common.Entity
{
    /// <summary>
    /// 微信搜索政策大卡-招生政策-日程
    /// </summary>
    public class WeChatRecruitScheduleInfo
    {
        [Identity(false)]
        public Guid ID { get; set; }
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
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 数据Json
        /// </summary>
        [JsonIgnore]
        public string DataJson { get; set; }
        [Display(IsField = false)]
        public IEnumerable<WeChatRecruitScheduleItem> DataJson_Obj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(DataJson)) return DataJson.FromJsonSafe<IEnumerable<WeChatRecruitScheduleItem>>();
                return default;
            }
        }
        /// <summary>
        /// 数据Json内最小时间
        /// </summary>
        public DateTime? MinDate { get; set; }
        /// <summary>
        /// 数据Json内最大时间
        /// </summary>
        public DateTime? MaxDate { get; set; }
        [Display(IsField = false)]
        public long MaxTime
        {
            get
            {
                if (MaxDate.HasValue) return MaxDate.Value.ToUnixTimestampByMilliseconds();
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
    public class WeChatRecruitScheduleItem
    {
        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        public long StartTime
        {
            get
            {
                return StartDate.ToUnixTimestampByMilliseconds();
            }
        }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
        public long EndTime
        {
            get
            {
                return EndDate.ToUnixTimestampByMilliseconds();
            }
        }
        /// <summary>
        /// 日程内容
        /// </summary>
        public string Content { get; set; }
    }
}
