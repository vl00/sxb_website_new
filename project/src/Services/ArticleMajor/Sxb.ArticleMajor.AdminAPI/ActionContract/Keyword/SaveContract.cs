using Sxb.ArticleMajor.Common.Enum;
using System.ComponentModel.DataAnnotations;

namespace Sxb.ArticleMajor.AdminAPI.ActionContract.Keyword
{
    public class SaveRequest
    {
        /// <summary>
        /// ID , 不传则表示新增
        /// </summary>
        public Guid? ID { get; set; }
        /// <summary>
        /// 站点类型
        /// </summary>
        public KeywordSiteType? SiteType { get; set; }
        /// <summary>
        /// 城市代码
        /// </summary>
        public int? CityCode { get; set; }
        /// <summary>
        /// 页面类型
        /// </summary>
        public KeywordPageType? PageType { get; set; }
        /// <summary>
        /// 未知类型
        /// </summary>
        public KeywordPositionType? PositionType { get; set; }
        /// <summary>
        /// 数据JSON
        /// </summary>
        [Required]
        public string DataJson { get; set; }
    }
}
