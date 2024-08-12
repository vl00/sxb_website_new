using Newtonsoft.Json;
using Sxb.School.Common.DTO;
using System;
using System.Collections.Generic;

namespace Sxb.School.API.RequestContact.CallSchoolApi
{    
    public class GetSchoolsIdAndNameQryResult
    {        
        /// <summary>
        /// 学校
        /// </summary>
        public List<SchoolIdAndNameDto> Items { get; set; }
    }


}
