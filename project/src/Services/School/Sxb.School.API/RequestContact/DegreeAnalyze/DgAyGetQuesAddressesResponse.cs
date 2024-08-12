using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.School.API.RequestContact.DegreeAnalyze
{
    public class DgAyGetQuesAddressesResponse
    {
        ///// <summary>/// 选项id/// </summary>
        //public Guid Id { get; set; }

        /// <summary>
        /// 地区编码
        /// </summary>
        public long Area { get; set; }
        /// <summary>
        /// 房产地址s
        /// </summary>
        public List<string> Address { get; set; }
    }
}
