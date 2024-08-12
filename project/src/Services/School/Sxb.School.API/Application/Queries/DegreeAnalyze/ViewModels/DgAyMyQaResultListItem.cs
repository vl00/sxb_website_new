using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sxb.School.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.School.API.Application.Queries.DegreeAnalyze
{
    /// <summary>
    /// 报告列表项
    /// </summary>
    public class DgAyMyQaResultListItem
    {
        /// <summary>
        /// 报告id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 解锁时间
        /// </summary>
        [JsonConverter(typeof(DateTimeToTimestampJsonConverter))]
        public DateTime Time { get; set; }
    }
}
