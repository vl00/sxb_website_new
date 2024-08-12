using Newtonsoft.Json;
using Sxb.School.Common.DTO;
using System;
using System.Collections.Generic;

namespace Sxb.School.API.RequestContact.School
{    
    public class QueryGeneralCommentByEidsResponse
    {        
        /// <summary>
        /// 
        /// </summary>
        public List<SchoolGeneralCommentDto> Items { get; set; }
    }

}
