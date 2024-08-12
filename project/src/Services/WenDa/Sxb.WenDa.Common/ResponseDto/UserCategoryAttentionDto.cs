using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    /// <summary>
    /// 用户关注的分类 (擅长领域)
    /// </summary>
    public class UserCategoryAttentionDto
    {
        /// <summary>
        /// 一级分类
        /// </summary>
        public List<CategoryDto> Categories1 { get; set; }
        /// <summary>
        /// 二级分类s <br/>
        /// 第一层数组的index与一级分类的index对应
        /// </summary>
        public List<List<CategoryDto>> Categories2 { get; set; }
    }
}
