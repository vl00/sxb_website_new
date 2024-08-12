using System;

namespace Sxb.School.API.RequestContact.School
{
    public class GetFractions2Request
    {
        /// <summary>
        /// 学部ID
        /// </summary>
        public Guid EID { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int? Year { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int? Type { get; set; }
    }
}
