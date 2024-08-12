using Sxb.ArticleMajor.Common.Enum;
using System;
using System.Collections.Generic;

namespace Sxb.ArticleMajor.API.Application.Query.ViewModel
{
    public class ArticleDetailVM
    {
        public string Id { get; set; }

        /// <summary> 
        /// </summary> 
        public string Code { get; set; }

        /// <summary> 
        /// </summary> 
        public string Title { get; set; }

        /// <summary> 
        /// </summary> 
        public string Author { get; set; }

        /// <summary> 
        /// </summary> 
        public string FromWhere { get; set; }

        /// <summary> 
        /// </summary> 
        public string PublishTime { get; set; }
        public ArticlePlatform Platform { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 总页面
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 类型  中考报考 > 中考分数线 > 2021中考分数线 > 正文
        /// </summary>
        public IEnumerable<ArticleCategoryVM> Categories { get; set; }
    }
}
