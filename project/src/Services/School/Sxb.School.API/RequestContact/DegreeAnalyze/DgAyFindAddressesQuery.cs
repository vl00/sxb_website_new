using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.School.API.RequestContact.DegreeAnalyze
{
    public class DgAyFindAddressesQuery
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        /// <summary>
        /// 地区编码
        /// </summary>
        public long Area { get; set; }
        /// <summary>
        /// 用于搜索地址
        /// </summary>
        public string Kw { get; set; }
    }
}
