using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    public class LoadAnswerForSaveDto
    {
        /// <summary>回答id</summary>
        public Guid Id { get; set; }

        /// <summary>富文本内容</summary>
        public string Content { get; set; }

        /// <summary>图片s</summary>
        public string[] Imgs { get; set; }
        /// <summary>图片缩略图s</summary>
        public string[] Imgs_s { get; set; }

        /// <summary>
        /// true=匿名; false=非匿名
        /// </summary>
        public bool IsAnony { get; set; }
    }
}
