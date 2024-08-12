using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    /// <summary>
    /// 分类dto
    /// </summary>
    public class CategoryDto
    {
        /// <summary>id</summary>
        public long Id { get; set; }
        /// <summary>名</summary>
        public string Name { get; set; }
        /// <summary>是否选中</summary>
        public bool? Selected { get; set; }
        /// <summary>
        /// 是否需要查学校. <br/>
        /// 分类=学校问答 时此值为true.
        /// </summary>
        public bool? CanFindSchool { get; set; }
    }

    /// <summary>
    /// 分类标签dto
    /// </summary>
    public class CategoryTagDto : CategoryDto { }
}
