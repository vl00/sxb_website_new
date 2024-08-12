using System;

namespace Sxb.School.API.Application.Models
{
    public record CreateDgAyOrderScanCallBackAttachData
    {
        public Guid OrderId { get; set; }

        /// <summary>
        /// 1 -> 免费 2->仅解锁结果 3->既解锁结果又解锁A级学校
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// 下单终端类型 1->h5 2->pc 3->小程序
        /// </summary>
        public byte Termtyp { get; set; }

    }
}
