using Kogel.Dapper.Extension.Attributes;
using Sxb.ArticleMajor.Common.Enum;
using System;

namespace Sxb.ArticleMajor.Common.Entity
{
    /// <summary>
    /// 关键词
    /// </summary>
    public class KeywordInfo
    {
        [Identity(false)]
        public Guid ID { get; set; }
        public KeywordSiteType? SiteType { get; set; }
        public int? CityCode { get; set; }
        public KeywordPageType? PageType { get; set; }
        public KeywordPositionType? PositionType { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifyTime { get; set; }
        public string DataJson { get; set; }
    }
}
