using System;
using System.Collections.Generic;

namespace Sxb.School.API.RequestContact.School
{
    public class GetCounterPartRequest
    {
        /// <summary>
        /// 学部ID
        /// </summary>
        public Guid EID { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int? Year { get; set; }
    }
    public class GetCounterPartResponse
    {
        /// <summary>
        /// 对口学校
        /// </summary>
        public IEnumerable<(string, string, string, string)> CounterPart { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Years { get; set; }
    }
}
