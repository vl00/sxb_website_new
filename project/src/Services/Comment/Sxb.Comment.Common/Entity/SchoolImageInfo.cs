using Kogel.Dapper.Extension.Attributes;
using Sxb.Comment.Common.Enum;
using System;

namespace Sxb.Comment.Common.Entity
{
    [Display(Rename = "SchoolImage")]
    public class SchoolImageInfo
    {
        public SchoolImageInfo()
        {
            ID = Guid.NewGuid();
        }

        [Identity]
        public Guid ID { get; set; }
        /// <summary>
        /// 所属数据ID
        /// </summary>
        public Guid DataSourcetID { get; set; }
        /// <summary>
        /// 上传图片类型
        /// </summary>
        public ImageType ImageType { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }
        public DateTime AddTime { get; set; }
    }
}
