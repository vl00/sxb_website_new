using System;

namespace Sxb.School.API.Application.Models
{
    public record ViewSchoolScanCallBackAttachData
    {
        public Guid OrderId { get; set; }

        /// <summary>
        /// 1 免费
        /// 2 付费
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FW { get; set; }

    }
}
