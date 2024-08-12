using Sxb.School.Common.OtherAPIClient.Article.Model.Entity;
using System;
using System.Collections.Generic;

namespace Sxb.School.Common.OtherAPIClient.Article.Model
{
    public class ListOrgLessonRequest
    {
        /// <summary>
        /// 城市Code
        /// </summary>
        public int CityID { get; set; }
        /// <summary>
        ///区域Code
        /// </summary>
        public int AreaID { get; set; }
        /// <summary>
        /// 学校ID
        /// </summary>
        public Guid SID { get; set; }
        /// <summary>
        /// 学部Id
        /// </summary>
        public Guid EID { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public string SchFType { get; set; }
        /// <summary>
        /// 学校学段
        /// </summary>
        public int Grade { get; set; }
    }
    public class ListOrgLessonResponse
    {
        public IEnumerable<HotSellCourse> Lessons { get; set; }
        public IEnumerable<RecommendOrg> Orgs { get; set; }
    }
}
