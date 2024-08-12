using Sxb.Framework.Foundation;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sxb.School.Common.DTO
{
    public class DgAyUserQaPaperDto : DgAyUserQaPaper
    {
        public string Ctn { get; set; }
    }

    public class DgAyUserQaPaperIsUnlockedDto
    {
        /// <summary>
		/// 
		/// </summary> 
		public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public Guid UserId { get; set; }

        /// <summary>
		/// 1=做题阶段，2=已分析出结果, 3=已解锁
		/// </summary> 
		public byte Status { get; set; }

        /// <summary>
		/// 1=免费解锁, 2=解锁x元, 3=解锁1元, 0=无结果变解锁
		/// </summary> 
		public byte? UnlockedType { get; set; }

        /// <summary>
        /// 解锁时间
        /// </summary> 
        public DateTime? UnlockedTime { get; set; }
    }
}
