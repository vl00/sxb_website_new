using Sxb.Framework.Foundation;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sxb.School.Common.DTO
{
    public class DgAySchoolItemDto
    {
        public Guid Eid { get; set; }
        /// <summary>学部短id</summary>
        public long No { get; set; }
        /// <summary>学校名</summary>
        public string Schname { get; set; }
        /// <summary>学部名</summary>
        public string Extname { get; set; }
        /// <summary>地址</summary>
        public string Address { get; set; }

        public string SchFtype { get; set; }
        public int EduSysType { get; set; }

        /// <summary>
        /// 招生方式
        /// </summary>
        public string RecruitWay { get; set; }

        public string Authentication { get; set; }

        public bool IsValid { get; set; }
    }
}
