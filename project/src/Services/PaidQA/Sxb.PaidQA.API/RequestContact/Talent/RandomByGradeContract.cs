using System;

namespace Sxb.PaidQA.API.RequestContact.Talent
{
    public class RandomByGradeRequest
    {
        /// <summary>
        /// 学段
        /// </summary>
        public int Grade { get; set; }
        /// <summary>
        /// 是否内部达人
        /// </summary>
        public bool IsInteral { get; set; } = true;
        /// <summary>
        /// 学部ID
        /// </summary>
        public Guid? EID { get; set; }
    }
}
