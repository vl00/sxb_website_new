using Newtonsoft.Json;
using Sxb.School.Common.DTO;
using System;
using System.Collections.Generic;

namespace Sxb.School.Common.DTO
{    
    /// <summary>
    /// (首页)学校总评
    /// </summary>
    public class SchoolGeneralCommentDto
    {
        [JsonIgnore]
        public Guid Eid { get; set; }

        /// <summary>
        /// 学校名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 标签s
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
		/// 学校简介
		/// </summary> 
		public string Intro { get; set; }
        /// <summary>
        /// 学校logo
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// (家长评价)共n人点评
        /// </summary>
        public int? CommentCount { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public SchoolScoreTreeDTO SchoolScore { get; set; }

    }

}
