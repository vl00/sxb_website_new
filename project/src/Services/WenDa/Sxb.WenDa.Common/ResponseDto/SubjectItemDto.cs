using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    /// <summary>
    /// 专栏item dto
    /// </summary>
    public class SubjectItemDto
    {
        /// <summary>专栏id</summary>
        public Guid SubjectId { get; set; }
        /// <summary>专栏短id</summary>
        public string SubjectNo { get; set; }
        /// <summary>专栏名/标题</summary>
        public string Title { get; set; }
        /// <summary>专栏简介</summary>
        public string Content { get; set; }

        /// <summary>原图</summary>
        public string Img { get; set; }
        /// <summary>缩略图</summary>
        public string Img_s { get; set; }

        /// <summary>浏览数</summary>
        public int ViewCount { get; set; }

        /// <summary>收藏数</summary>
        public int CollectCount { get; set; }
        /// <summary>true=我有收藏</summary>
        public bool? IsCollectedByMe { get; set; }

        /// <summary>分类ids</summary>
        public long[] CategoryIds { get; set; }
        /// <summary>分类名s</summary>
        public string[] CategoryNames { get; set; }
        /// <summary>标签ids</summary>
        public long[] TagIds { get; set; } = null;
        /// <summary>标签名s</summary>
        public string[] TagNames { get; set; } = null;
    }
}
