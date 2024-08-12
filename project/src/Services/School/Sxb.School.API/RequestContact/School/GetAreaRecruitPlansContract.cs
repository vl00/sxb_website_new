using System;

namespace Sxb.School.API.RequestContact.School
{
    public class GetAreaRecruitPlansRequest
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
}
