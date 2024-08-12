using Newtonsoft.Json;
using Sxb.Framework.Foundation;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;

namespace Sxb.School.Common.DTO
{
    public class SchoolIdAndNameDto
    {
        /// <summary>学校eid</summary>
        public Guid Eid { get; set; }
        public long Eno { get; set; }
        /// <summary>学校短id</summary>
        public string SchoolNo
        {
            get
            {
                try { return UrlShortIdUtil.Long2Base32(Eno); }
                catch { return null; }
            }
        }
        /// <summary>学校名</summary>
        public string Schname { get; set; }
        /// <summary>学部名</summary>
        public string Extname { get; set; }
        /// <summary>true=正常; false=已删除</summary>
        public bool IsValid { get; set; }
    }
}
