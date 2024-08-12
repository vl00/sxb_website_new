using Sxb.School.Common.Enum;
using System;

namespace Sxb.School.API.RequestContact.School
{
    public class GetFractionsRequest
    {
        /// <summary>
        /// 学部ID
        /// </summary>
        public Guid EID { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int? Year { get; set; }
        public ExtensionFractionType Type { get; set; }
    }
}
