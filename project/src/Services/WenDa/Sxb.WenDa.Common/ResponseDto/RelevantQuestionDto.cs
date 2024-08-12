using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    /// <summary>
    /// 相关问题 推荐问题 dto
    /// </summary>
    public class RelevantQuestionDto
    {
        /// <summary>问题id</summary>
        public Guid QuestionId { get; set; }
        /// <summary>问题短id</summary>
        public string QuestionNo { get; set; }
        /// <summary>问题标题</summary>
        public string Title { get; set; }

        ///// <summary>标签名</summary>
        //public string Category { get; set; } = null;

        /// <summary>后端字段，前端不管</summary>
        public long? _No { get; set; } = null;        
    }

    /// <summary>
    /// 相关推荐文章 (专栏页)
    /// </summary>
    public class RelevantArticleDto
    {
        /// <summary>文章短id</summary>
        public string ArticleNo { get; set; }

        /// <summary>文章标题</summary>
        public string Title { get; set; }
        ///// <summary>标签名</summary>
        //public string Category { get; set; }
    }
}
