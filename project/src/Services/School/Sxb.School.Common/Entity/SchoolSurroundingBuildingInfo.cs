using Kogel.Dapper.Extension.Attributes;
using System;

namespace Sxb.School.Common.Entity
{
    [Display(Rename = "SchoolSurroundingBuilding")]
    public class SchoolSurroundingBuildingInfo
    {
        [Identity(false)]
        public Guid ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string House_ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string House_Img { get; set; }
        /// <summary>
        /// 房产cityId
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double? House_Lat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double? House_Lng { get; set; }
        /// <summary>
        /// 房产年份（json）
        /// </summary>
        public string Building_Years { get; set; }
        /// <summary>
        /// 房均价 元/平米
        /// </summary>
        public string House_Price { get; set; }
        /// <summary>
        /// 房产所属物业（json）
        /// </summary>
        public string House_Properties { get; set; }
        /// <summary>
        /// 房产所属开发商（json）
        /// </summary>
        public string House_Developers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime { get; set; }
        [Display(IsField = false)]
        public double? Distance { get; set; }
        /// <summary>
        /// 去除水印部分的图片链接
        /// </summary>
        public string New_House_Img { get; set; }
    }
}
