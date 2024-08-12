using System;
using System.Collections.Generic;

namespace Sxb.School.API.RequestContact.School
{
    public class GetExtSimpleInfoRequest
    {
        /// <summary>
        /// 学部ID
        /// </summary>
        public IEnumerable<Guid> EIDs { get; set; }
    }
}
