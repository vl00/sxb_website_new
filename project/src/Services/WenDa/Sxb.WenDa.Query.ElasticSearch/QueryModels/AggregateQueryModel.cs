using Newtonsoft.Json;
using Sxb.WenDa.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Query.ElasticSearch.QueryModels
{
    public class AggregateQueryModel
    {
        /// <summary>
        /// 类型  1 问题  3 专栏
        /// </summary>
        public RefTable? Type { get; set; }

        /// <summary>
        /// 输入的关键词
        /// </summary>
        public string? Keyword { get; set; }

        public long? City { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
