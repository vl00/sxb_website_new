using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Recommend.API.RequestModels
{
    public class IdRedirectAddRequestDto
    {
        /// <summary>
        /// 源学校页面的学部id
        /// </summary>
        public Guid ReferId { get; set; }

        /// <summary>
        /// 目标学校页面的学部id
        /// </summary>
        public Guid CurrentId { get; set; }
    }

    public class ShortIdRedirectAddRequestDto
    {
        /// <summary>
        /// 源学校页面的学部id
        /// </summary>
        public string ReferShortId { get; set; }

        /// <summary>
        /// 目标学校页面的学部id
        /// </summary>
        public string CurrentShortId { get; set; }
    }
}
