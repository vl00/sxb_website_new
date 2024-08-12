using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Query.ElasticSearch.Models
{

    public class SearchSubject : BaseEsModel
    {
        /// <summary>
        /// 
        /// </summary> 
        public long No { get; set; }

        /// <summary>
        /// 标题
        /// </summary> 
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary> 
        public string Content { get; set; }

        /// <summary>
        /// 浏览数
        /// </summary> 
        public int ViewCount { get; set; } = 0;

        /// <summary>
        /// 收藏数
        /// </summary> 
        public int CollectCount { get; set; } = 0;

        /// <summary>
        /// 1=正常 0=已删除
        /// </summary> 
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// 创建时间
        /// </summary> 
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary> 
        public DateTime ModifyDateTime { get; set; }
    }
}
