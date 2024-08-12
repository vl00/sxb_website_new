using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Common.ResponseDto
{
    /// <summary>
    /// 用于load编辑问题
    /// </summary>
    public class LoadQuestionForSaveDto
    {
        /// <summary>
        /// 问题id
        /// </summary>
        public Guid? Id { get; set; } = default!;
        /// <summary>问题标题</summary>
        public string Title { get; set; }
        /// <summary>问题补充说明</summary>
        public string Content { get; set; }
        /// <summary>图片s</summary>
        public string[] Imgs { get; set; }
        /// <summary>图片缩略图s</summary>
        public string[] Imgs_s { get; set; }

        /// <summary>城市</summary>
        [Required]
        public LoadQuestionForSaveDto_City[] Citys { get; set; } = default!;

        /// <summary>
        /// 分类 <br/>
        /// 数组顺序=分类级别顺序
        /// </summary>
        public List<CategoryDto[]> Categories { get; set; }

        /// <summary>学校</summary>
        public LoadQuestionForSaveDto_School[] Schools { get; set; }

        /// <summary>标签</summary>
        public CategoryTagDto[] Tags { get; set; }

        /// <summary>
        /// true=匿名; false=非匿名
        /// </summary>
        public bool IsAnony { get; set; }
    }

    public class LoadQuestionForSaveDto_City
    {
        /// <summary>城市编码</summary>
        public long Id { get; set; }
        /// <summary>城市名</summary>
        public string Name { get; set; }
        /// <summary>是否选中</summary>
        public bool? Selected { get; set; }
    }
    public class LoadQuestionForSaveDto_School
    {
        /// <summary>学校eids</summary>
        public Guid Eid { get; set; }
        /// <summary>学校名</summary>
        public string SchoolName { get; set; }
    }
}
