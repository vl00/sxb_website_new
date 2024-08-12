using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Value
{
    public class SchoolFilterDefinition : ValueObject
    {

        /// <summary>
        /// 定位城市
        /// </summary>
        public int LocationCity { get; set; }

        /// <summary>
        /// 筛选区
        /// </summary>
        public int Area { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 学校评分
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 学校认证
        /// </summary>
        public IEnumerable<string> Authentication { get; set; }

        /// <summary>
        /// 课程设置
        /// </summary>
        public IEnumerable<string> CourseSetting { get; set; }

        /// <summary>
        /// 特色课程
        /// </summary>
        public IEnumerable<string> SpecialCourse { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            return new List<object>() { this.LocationCity, this.Type, this.Score, this.Area, this.Authentication, this.CourseSetting, this.SpecialCourse };
        }
    }
}
