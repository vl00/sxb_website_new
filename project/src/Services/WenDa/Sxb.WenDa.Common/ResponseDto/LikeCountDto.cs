using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class LikeCountDto
    {
        /// <summary>id</summary>
        public Guid Id { get; set; }
        /// <summary>点赞数</summary>
        public int LikeCount { get; set; }
        /// <summary>是否我点赞过的</summary>
        public bool IsLikeByMe { get; set; }
    }
}
