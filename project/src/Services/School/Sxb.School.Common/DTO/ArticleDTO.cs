using System;
using System.Collections.Generic;

namespace Sxb.School.Common.DTO
{
    /// <summary>
    /// 文章
    /// </summary>
    public class ArticleDTO
    {
        /// <summary>
        /// 标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 发表时间(中文)
        /// </summary>
        public string CNTime
        {
            get
            {
                if (DateTime.TryParse(Time, out DateTime convertResult)) return convertResult.ToString("yyyy年MM月dd日");
                return Time;
            }
        }

        /// <summary>
        /// 阅读量
        /// </summary>
        public int ViweCount { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 点击跳转的链接
        /// </summary>
        public string ActionUrl { get; set; }
        /// <summary>
        /// 背景图片
        /// </summary>
        public IEnumerable<string> Covers { get; set; }
        /// <summary>
        /// 列表布局方式
        /// </summary>
        public int Layout { get; set; }
        /// <summary>
        /// 文章摘要
        /// </summary>
        public string Digest { get; set; }
        public string No { get; set; }
    }
}
