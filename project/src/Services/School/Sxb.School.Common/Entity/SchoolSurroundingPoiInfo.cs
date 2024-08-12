using Kogel.Dapper.Extension.Attributes;
using Sxb.School.Common.Enum;
using System;

namespace Sxb.School.Common.Entity
{
    [Display(Rename = "SchoolSurroundingPoi")]
    public class SchoolSurroundingPoiInfo
    {
        [Identity(false)]
        public Guid ID { get; set; }
        /// <summary>
        /// poi对应的唯一id
        /// </summary>
        public string Poi_ID { get; set; }
        /// <summary>
        /// 0:其它
        /// 1:商场
        /// 2:书店
        /// 3:医院
        /// 4:警察局
        /// 5:地铁
        /// 6:公交车
        /// 7:房产相关
        /// </summary>
        public SchoolSurroundingPoiType TypeCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Poi_Province { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Poi_City { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Poi_Area { get; set; }
        /// <summary>
        /// poi对应tag（多个）（json）
        /// </summary>
        public string Poi_Tags { get; set; }
        /// <summary>
        /// poi对应的纬度
        /// </summary>
        public double? Poi_Lat { get; set; }
        /// <summary>
        /// poi对应的经度
        /// </summary>
        public double? Poi_Long { get; set; }
        /// <summary>
        /// poi名称
        /// </summary>
        public string Poi_Name { get; set; }
        /// <summary>
        /// poi对应的图片（json）
        /// </summary>
        public string Poi_Photo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// poi地址、车站站点、地铁途径线路等（json）
        /// </summary>
        public string Poi_Address { get; set; }
        /// <summary>
        /// poi房产价格 元/平方米
        /// </summary>
        public string Poi_Price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(IsField = false)]
        public double? Distance { get; set; }
    }
}
